package net.sourceforge.cruisecontrol.sourcecontrols;

import java.io.*;

import java.text.*;
import java.util.*;
import net.sourceforge.cruisecontrol.Modification;
import net.sourceforge.cruisecontrol.SourceControl;
import net.sourceforge.cruisecontrol.util.StreamPumper;

import org.apache.log4j.Category;

/**
*  This class implements the SourceControlElement methods for a PVCS
*  repository.
*
*  @author <a href="mailto:Richard.Wagner@alltel.com">Richard Wagner</a>
*/
public class PVCS implements SourceControl {

	/** enable logging for this class */
	private static Category log = Category.getInstance(PVCS.class.getName());

	private Hashtable _properties = new Hashtable();
	private String _property;
	private String _propertyOnDelete;

	private String _pvcsProject;
	// i.e. "esa";
	// i.e. "esa/uihub2";
	private String _pvcsSubProject;

	/**
	*  Date format required by commands passed to PVCS
	*/
	final static SimpleDateFormat IN_DATE_FORMAT = new SimpleDateFormat("MM/dd/yyyy/HH:mm");

	/**
	*  Date format returned in the output of PVCS commands.
	*/
	final static SimpleDateFormat OUT_DATE_FORMAT = new SimpleDateFormat("MMM dd yyyy HH:mm:ss");



	/**
	*  Some constants used in file-based PVCS interactions.
	*/
	final static String PVCS_INSTRUCTIONS_FILE = "CruiseControlPVCS.pcli";
	final static String PVCS_TEMP_WORK_FILE = "files.tmp";
	final static String PVCS_RESULTS_FILE = "";




	public void setPvcsproject(String project) {
		_pvcsProject = project;
	}
	public void setPvcssubproject(String subproject) {
		_pvcsSubProject = subproject;
	}

	public void setProperty(String property) {
		_property = property;
	}

	public void setPropertyOnDelete(String propertyOnDelete) {
		_propertyOnDelete = propertyOnDelete;
	}

	public Hashtable getProperties() {
		return _properties;
	}

	/**
	*  Returns an {@link java.util.List List} of {@link Modification}
	*  detailing all the changes between now and the last build.
	*
	*@param  lastBuild the last build time
	*@param  now time now, or time to check
	*@return  the list of modifications, an empty (not null) list if no
	*      modifications or if developer had checked in files since quietPeriod seconds ago.
	*
	*  Note:  Internally uses external filesystem for files CruiseControlPVCS.pcli, files.tmp, vlog.txt
	*/
	public List getModifications(Date lastBuild, Date now) {
		// build file of PVCS command line instructions
		String lastBuildDate = IN_DATE_FORMAT.format(lastBuild);
		String nowDate = IN_DATE_FORMAT.format(now);
		buildExecFile(lastBuildDate, nowDate);
		String command = "pcli run -sCruiseControlPVCS.pcli";
		List modifications = null;
		try {
			int exitValue;
			Process p = Runtime.getRuntime().exec(command);
			StreamPumper errorPumper = new StreamPumper(p.getErrorStream());
			new Thread(errorPumper).start();
			InputStream input = p.getInputStream();
			p.waitFor();
		}
		catch (Exception e) {
			log.error("Error in executing the PVCS command : ", e);
			return new ArrayList();
		}
		modifications = makeModificationsList();

		if (modifications == null) {
			modifications = new ArrayList();
		}

		return modifications;
	}


	/**
	*  Read the file produced by PCLI listing all changes to the source repository
	*  Once we've read the file, produce a list of changes.
	*/
	private List makeModificationsList(){
		List theList = new ArrayList();
		File inputFile = new File("vlog.txt");
		BufferedReader brIn;
		ModificationBuilder modificationBuilder = new ModificationBuilder();
		try{
			brIn = new BufferedReader(new FileReader(inputFile));
			String line;
			while ((line = brIn.readLine()) != null){
				modificationBuilder.addLine(line);
			}
			brIn.close();
		}
		catch(IOException e){
			log.error("Error in reading vlog file of PVCS modifications : ", e);
		}
		theList = modificationBuilder.getList();
		return theList;
	}

	/**
	*  Builds a file of PVCS instructions to execute.  The format should be roughly:
	*
	*  set -vProject "-prv:\esa"
	*  set -vSubProject "/esa/uihub2"
	*  Echo Getting list of files
	*  run ->files.tmp listversionedfiles -z -aw $Project $SubProject
	*  Echo Getting History
	*  run -e vlog  "-xo+evlog.txt" "-d07/20/2001/10:49*07/30/2001" "@files.tmp"
	*
	*/
	private void buildExecFile(String lastBuild,String now){
		File outputFile = new File(PVCS_INSTRUCTIONS_FILE);

		String forwardSlash = "/";
		String backSlash = "\\";
		String doubleQuotes = "\"";
		String atSign = "@";

		String line1 = "set -vProject " + doubleQuotes + "-pr" + _pvcsProject + doubleQuotes;
		String line2 = "set -vSubProject " + doubleQuotes + _pvcsSubProject + doubleQuotes;
		String line3 = "run ->files.tmp listversionedfiles -z -aw $Project $SubProject";
		String line4Subline1 = "run -e vlog " + doubleQuotes + "-xo+evlog.txt" + doubleQuotes + " ";
		String line4Subline2 =  doubleQuotes + "-d" + lastBuild + "*" + now + doubleQuotes + " ";
		String line4Subline3 =  doubleQuotes + atSign + PVCS_TEMP_WORK_FILE + doubleQuotes;
		String line4 = line4Subline1 + line4Subline2 + line4Subline3;

		log.debug("#### PVCSElement about to write this line:\n " + line4Subline2);

		BufferedWriter bwOut;
		try{
			bwOut = new BufferedWriter(new FileWriter(outputFile));
			bwOut.write(line1);
			bwOut.write("\n");
			bwOut.write(line2);
			bwOut.write("\n");
			bwOut.write(line3);
			bwOut.write("\n");
			bwOut.write(line4);
			bwOut.write("\n");
			bwOut.close();
		}
		catch(IOException e){
			log.error("Error in building PVCS pcli file : ", e);
		}
	}

	/**
	*  Inner class to build Modifications and verify the order of the lines
	*   used to build them.
	*/
	class ModificationBuilder {

		private Modification modification;
		private ArrayList modifications = null;
		private String lastLine = null;
		private boolean firstModifiedTime = true;
		private boolean firstUserName     = true;
		private boolean nextLineIsComment = false;
		private boolean waitingForNextValidStart = false;

		public ArrayList getList(){
			return modifications;
		}

		private void initializeModification(){
			if (modifications == null){
				modifications = new ArrayList();
			}
			modification = new Modification();
			firstModifiedTime = true;
			firstUserName = true;
			nextLineIsComment = false;
			waitingForNextValidStart = false;
		}

		public void addLine(String line){
			if (line.startsWith("Archive:")){
				initializeModification();
			}
			else if (waitingForNextValidStart){
				// we're in this state after we've got the last useful line
				// from the previous item, but haven't yet started a new one
				// -- we should just skip these lines till we start a new one
				return;
			}
			else if (line.startsWith("Workfile:")){
				modification.fileName = line.substring(18);
			}
			else if (line.startsWith("Last modified:")){
				// if this is the newest revision...
				if (firstModifiedTime){
					firstModifiedTime = false;
					try {
						String lastMod = line.substring(16);
						modification.modifiedTime = OUT_DATE_FORMAT.parse(lastMod);
					}
					catch (ParseException e) {
						modification.modifiedTime = null;
					}
				}
			}
			else if (nextLineIsComment == true){
				// used boolean because don't know what comment will startWith....
				modification.comment = line;
				// comment is last line we need, so add this mod to list,
				//  then set indicator to ignore future lines till next new item
				modifications.add(modification);
				waitingForNextValidStart = true;
			}
			else if (line.startsWith("Author id:")){
				// if this is the newest revision...
				if (firstUserName){
					String sub = line.substring(11);
					StringTokenizer st = new StringTokenizer(sub, " ");
					String username = st.nextToken().trim();
					modification.userName = username;
					firstUserName = false;
					nextLineIsComment = true;
				}
			}  // end of Author id


		}   // end of addLine
	}  // end of class ModificationBuilder
}  // end class PVCSElement
