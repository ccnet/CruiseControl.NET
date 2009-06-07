using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
    [TestFixture]
    public class VstsHistoryParserTest : CustomAssertion
    {
        string emptyLog = "No history entries were found for the item and version combination specified";
        string singleRemoveLog = "-------------------------------------------------------------------------------\n" +
                          "Changeset: 996969\n" +
                          "User: s.user\n" + 
                          "Date: Thursday, May 14, 2009 5:47:53 PM\n" +
                          "\n" +
                          "Comment:\n" +
                          "  Delete unused directory\n" +
                          "\n" +
                          "Items:\n" +
                          "  delete $/maintrunk/Dev/somefolder/folder;X13366\n" +
                          "\n" +
                          "Check-in Notes:\n" +
                          "  Code Reviewer:\n" +
                          "  Performance Reviewer:\n" + 
                          "  Security Reviewer:\n\n";
        string singleAddLog = "-------------------------------------------------------------------------------\n" +
                          "Changeset: 996969\n" +
                          "User: s.user\n" + 
                          "Date: Thursday, May 14, 2009 5:47:53 PM\n" +
                          "\n" +
                          "Comment:\n" +
                          "  Add unused directory\n" +
                          "\n" +
                          "Items:\n" +
                          "  add $/maintrunk/Dev/somefolder/folder;X13366\n" +
                          "\n" +
                          "Check-in Notes:\n" +
                          "  Code Reviewer:\n" +
                          "  Performance Reviewer:\n" + 
                          "  Security Reviewer:\n\n";
        string singleEditLog = "-------------------------------------------------------------------------------\n" +
                          "Changeset: 996969\n" +
                          "User: s.user\n" + 
                          "Date: Thursday, May 14, 2009 5:47:53 PM\n" +
                          "\n" +
                          "Comment:\n" +
                          " edit Change Directory\n" +
                          "\n" +
                          "Items:\n" +
                          "  edit $/maintrunk/Dev/somefolder/folder;X13366\n" +
                          "\n" +
                          "Check-in Notes:\n" +
                          "  Code Reviewer:\n" +
                          "  Performance Reviewer:\n" + 
                          "  Security Reviewer:\n\n";
        string fullLog = "-------------------------------------------------------------------------------\n" +
                        "Changeset: 110558\n" +
                        "User: s.user\n" +
                        "Date: Thursday, May 07, 2009 2:31:10 PM\n" +
                        "\n" +
                        "Comment:\n" +
                        "  Fix GDI leak\n" +
                        "\n" +
                        "Items:\n" +
                        "  edit $/Project/Dev/Src/WndUtil.cpp\n" +
                        "\n" +
                        "Check-in Notes:\n" +
                        "  Code Reviewer:\n" +
                        "  Performance Reviewer:\n" +
                        "  Security Reviewer:\n" +
                        "\n" +
                        "-------------------------------------------------------------------------------\n" +
                        "Changeset: 110551\n" +
                        "User: s.otheruser\n" +
                        "Date: Thursday, May 07, 2009 1:47:46 PM\n" +
                        "\n" +
                        "Comment:\n" +
                        "  Cleanup tests a little\n" +
                        "\n" +
                        "Items:\n" +
                        "  delete $/Project/Dev/Misc/MFCcharacterCaptureTest/MFCcharacterCaptureTest/characterMapTests.cpp\n" +
                        "\n" +
                        "Check-in Notes:\n" +
                        "  Code Reviewer:\n" +
                        "  Performance Reviewer:\n" +
                        "  Security Reviewer:\n" +
                        "\n" +
                        "-------------------------------------------------------------------------------\n" +
                        "Changeset: 110549\n" +
                        "User: s.otheruser\n" +
                        "Date: Thursday, May 07, 2009 1:37:53 PM\n" +
                        "\n" +
                        "Comment:\n" +
                        "  Some more RTF stub/psuedo code\n" +
                        "\n" +
                        "Items:\n" +
                        "  edit $/Project/Dev/Src/characterScalingStrategies.cpp\n" +
                        "\n" +
                        "Check-in Notes:\n" +
                        "  Code Reviewer:\n" +
                        "  Performance Reviewer:\n" +
                        "  Security Reviewer:\n" +
                        "\n" +
                        "-------------------------------------------------------------------------------\n" +
                        "Changeset: 110545\n" +
                        "User: s.otheruser\n" +
                        "Date: Thursday, May 07, 2009 1:03:27 PM\n" +
                        "\n" +
                        "Comment:\n" +
                        "  Get debug unit tests running again\n" +
                        "  build separate vc90.pdb files for each proj included in unit tests\n" +
                        "  RTF code stubs\n" +
                        "  Improved scaling for word fragments with spaces in them - required more accurate double char sizes rather than rounded int char sizes\n" +
                        "  Added unit tests for scaling probs that still exist\n" +
                        "  Added unit test to prove RegSetValue MSDN docs\n" +
                        "\n" +
                        "\n" +
                        "Items:\n" +
                        "  edit $/Project/Dev/Misc/MFCcharacterCaptureTest/MFCcharacterCaptureTest/MFCcharacterCaptureTest.vcproj\n" +
                        "  edit $/Project/Dev/Misc/MFCcharacterCaptureTest/MFCcharacterCaptureTest/MFCcharacterCaptureTestDlg.cpp\n" +
                        "  edit $/Project/Dev/Misc/MFCcharacterCaptureTest/MFCcharacterCaptureTest/characterMapTests.cpp\n" +
                        "  edit $/Project/Dev/Misc/MFCcharacterCaptureTest/MFCcharacterCaptureTest/ViewcharacterMapBinDlg.cpp\n" +
                        "  add  $/Project/Dev/Misc/MFCcharacterCaptureTest/MFCcharacterCaptureTest/WordExtraSpaces.bin\n" +
                        "  edit $/Project/Dev/ProjectUnitTests/CoreTests.cpp\n" +
                        "  edit $/Project/Dev/ProjectUnitTests/CoreTests.h\n" +
                        "  edit $/Project/Dev/ProjectUnitTests/characterCaptureTests.cpp\n" +
                        "  edit $/Project/Dev/projTxNT/SRC/APIHook.cpp\n" +
                        "  add  $/Project/Dev/projTxNT/SRC/Debug.cpp\n" +
                        "  add  $/Project/Dev/projTxNT/SRC/Debug.h\n" +
                        "  edit $/Project/Dev/projTxNT/SRC/RecordcharacterOut.cpp\n" +
                        "  edit $/Project/Dev/projTxNT/SRC/RecordcharacterOut.h\n" +
                        "  edit $/Project/Dev/projTxNT/SRC/projTxNT.cpp\n" +
                        "  edit $/Project/Dev/projTxNT/SRC/projTXNT.vcproj\n" +
                        "  edit $/Project/Dev/Src/DebugcharacterCapture.cpp\n" +
                        "  edit $/Project/Dev/Src/DebugcharacterCapture.h\n" +
                        "  edit $/Project/Dev/Src/IcharacterMap.h\n" +
                        "  edit $/Project/Dev/Src/Project.vcproj\n" +
                        "  edit $/Project/Dev/Src/characterCapture.cpp\n" +
                        "  add  $/Project/Dev/Src/characterCaptureHookCommon.h\n" +
                        "  edit $/Project/Dev/Src/characterCaptureStrategyWin32.cpp\n" +
                        "  edit $/Project/Dev/Src/characterCaptureStrategyWin32.h\n" +
                        "  edit $/Project/Dev/Src/characterMap.cpp\n" +
                        "  edit $/Project/Dev/Src/characterMap.h\n" +
                        "  edit $/Project/Dev/Src/characterScalingStrategies.cpp\n" +
                        "  edit $/Project/Dev/Src/characterScalingStrategies.h\n" +
                        "  edit $/Project/Project.sln\n" +
                        "\n" +
                        "Check-in Notes:\n" +
                        "  Code Reviewer:\n" +
                        "  Performance Reviewer:\n" +
                        "  Security Reviewer:\n" +
                        "\n" +
                        "-------------------------------------------------------------------------------\n" +
                        "Changeset: 110544\n" +
                        "User: s.otheruser\n" +
                        "Date: Thursday, May 07, 2009 12:58:38 PM\n" +
                        "\n" +
                        "Comment:\n" +
                        "    Remove Edit-and-Continue PDB files on some projs, we don't use that capability anyway and it just bloats the pdb files and causes extra build times - need to do it for all projs\n" +
                        "    Change output of vc90.pdb compiler pdb file to be {projname}vc90.pdb so that file for mult projs can exist in same dir, helping unit tests build \n" +
                        "\n" +
                        "Items:\n" +
                        "    edit $/Project/Dev/Add-ins/BrowserHelperObject/BHO.vcproj\n" +
                        "    edit $/Project/Dev/DataStore/DataStore.vcproj\n" +
                        "    edit $/Project/Dev/ExtensionArchitecture/ExtensionArchitecture.vcproj\n" +
                        "    edit $/Project/Dev/ProjectCommon/ProjectCommon.vcproj\n" +
                        "    edit $/Project/Dev/ProjectEditor/ProjectEditor.vcproj\n" +
                        "    edit $/Project/Dev/ProjectEditorRes/ProjectEditorRes.vcproj\n" +
                        "    edit $/Project/Dev/ProjectRes/ProjectRes.vcproj\n" +
                        "    edit $/Project/Dev/ProjectUnitTests/ProjectEditorUnitTestsEXE.vcproj\n" +
                        "    edit $/Project/Dev/ProjectUnitTests/ProjectUnitTestsEXE.vcproj\n" +
                        "    edit $/Project/Dev/projPriv/projPriv/projPriv.vcproj\n" +
                        "\n" +
                        "Check-in Notes:\n" +
                        "    Code Reviewer:\n" +
                        "    Performance Reviewer:\n" +
                        "    Security Reviewer:\n" +
                        "\n" +
                        "-------------------------------------------------------------------------------\n" +
                        "Changeset: 110531\n" +
                        "User: a.hutton\n" +
                        "Date: Thursday, May 07, 2009 10:59:21 AM\n" +
                        "\n" +
                        "Comment:\n" +
                        "   deleted config\n" +
                        "\n" +
                        "Items:\n" +
                        "    delete $/project/CruiseControl/ccnet.config\n" +
                        "\n" +
                        "Check-in Notes:\n" +
                        "    Code Reviewer:\n" +
                        "    Performance Reviewer:\n" +
                        "    Security Reviewer:\n" +
                        "\n" +
                        "-------------------------------------------------------------------------------\n" +
                        "Changeset: 110515\n" +
                        "User: a.anotheruser\n" +
                        "Date: Wednesday, May 06, 2009 5:13:35 PM\n" +
                        "\n" +
                        "Comment:\n" +
                        "    Updated the solution for the accessory sample applications to work with VS2008.\n" +
                        "\n" +
                        "Items:\n" +
                        "    edit $/Project/Dev/Extensions/Samples.sln\n" +
                        "    edit $/Project/Dev/Extensions/WPFOutput/WPFOutput.csproj\n" +
                        "\n" +
                        "Check-in Notes:\n" +
                        "    Code Reviewer:\n" +
                        "    Performance Reviewer:\n" +
                        "    Security Reviewer:\n" +
                        "\n" +
                        "-------------------------------------------------------------------------------\n" +
                        "Changeset: 110514\n" +
                        "User: s.user\n" +
                        "Date: Wednesday, May 06, 2009 5:12:32 PM\n" +
                        "\n" +
                        "Comment:\n" +
                        "    Refactor some window related functions into CWndUtil\n" +
                        "\n" +
                        "Items:\n" +
                        "    edit $/Project/Dev/Src/IMAGE.CPP\n" +
                        "    edit $/Project/Dev/Src/OldcharacterCapture.cpp\n" +
                        "    edit $/Project/Dev/Src/SCROLL.CPP\n" +
                        "    edit $/Project/Dev/Src/SELECT.CPP\n" +
                        "    delete $/Project/Dev/Src/SELECT.H\n" +
                        "    edit $/Project/Dev/Src/TopSecret.cpp\n" +
                        "    edit $/Project/Dev/Src/TopSecret.h\n" +
                        "    edit $/Project/Dev/Src/characterCapture.cpp\n" +
                        "    edit $/Project/Dev/Src/VIDEO.CPP\n" +
                        "    edit $/Project/Dev/Src/WndUtil.cpp\n" +
                        "    edit $/Project/Dev/Src/WndUtil.h\n" +
                        "\n" +
                        "Check-in Notes:\n" +
                        "    Code Reviewer:\n" +
                        "    Performance Reviewer:\n" +
                        "    Security Reviewer:\n" +
                        "\n" +
                        "-------------------------------------------------------------------------------\n" +
                        "Changeset: 110511\n" +
                        "User: s.user\n" +
                        "Date: Wednesday, May 06, 2009 4:37:56 PM\n" +
                        "\n" +
                        "Comment:\n" +
                        "    Add extra hardware checks for entering Direct3D mode.\n" +
                        "\n" +
                        "Items:\n" +
                        "    edit $/Project/Dev/Src/TopSecret.cpp\n" +
                        "\n" +
                        "Check-in Notes:\n" +
                        "    Code Reviewer:\n" +
                        "    Performance Reviewer:\n" +
                        "    Security Reviewer:\n" +
                        "\n" +
                        "-------------------------------------------------------------------------------\n" +
                        "Changeset: 110495\n" +
                        "User: a.user\n" +
                        "Date: Wednesday, May 06, 2009 2:16:26 PM\n" +
                        "\n" +
                        "Comment:\n" +
                        "    Japanese accessory help files from Matt\n" +
                        "\n" +
                        "Items:\n" +
                        "    add $/Project/_Branches/branch1/Project/Dev/Extensions/MSExcel//ProjectExcelOutput.chm\n" +
                        "    add $/Project/_Branches/branch1/Project/Dev/Extensions/MSPowerPoint/ProjectPowerPointOutput.chm\n" +
                        "    add $/Project/_Branches/branch1/Project/Dev/Extensions/MSWord//ProjectWordOutput.chm\n" +
                        "\n" +
                        "Check-in Notes:\n" +
                        "    Code Reviewer:\n" +
                        "    Performance Reviewer:\n" +
                        "    Security Reviewer:\n" +
                        "\n";
        string fullLogWithTooOld = "-------------------------------------------------------------------------------\n" +
                       "Changeset: 110558\n" +
                       "User: s.user\n" +
                       "Date: Tuesday, Apr 07, 2009 2:31:10 PM\n" +
                       "\n" +
                       "Comment:\n" +
                       "  Fix GDI leak\n" +
                       "\n" +
                       "Items:\n" +
                       "  edit $/Project/Dev/Src/WndUtil.cpp\n" +
                       "\n" +
                       "Check-in Notes:\n" +
                       "  Code Reviewer:\n" +
                       "  Performance Reviewer:\n" +
                       "  Security Reviewer:\n" +
                       "\n" +
                       "-------------------------------------------------------------------------------\n" +
                       "Changeset: 110551\n" +
                       "User: s.otheruser\n" +
                       "Date: Thursday, May 07, 2009 1:47:46 PM\n" +
                       "\n" +
                       "Comment:\n" +
                       "Cleanup tests a little\n" +
                       "\n" +
                       "Items:\n" +
                       "  delete $/Project/Dev/Misc/MFCcharacterCaptureTest/MFCcharacterCaptureTest/characterMapTests.cpp\n" +
                       "\n" +
                       "Check-in Notes:\n" +
                       "  Code Reviewer:\n" +
                       "  Performance Reviewer:\n" +
                       "  Security Reviewer:\n" +
                       "\n" +
                       "-------------------------------------------------------------------------------\n" +
                       "Changeset: 110549\n" +
                       "User: s.otheruser\n" +
                       "Date: Tuesday, Apr 07, 2009 1:37:53 PM\n" +
                       "\n" +
                       "Comment:\n" +
                       "  Some more RTF stub/psuedo code\n" +
                       "\n" +
                       "Items:\n" +
                       "  edit $/Project/Dev/Src/characterScalingStrategies.cpp\n" +
                       "\n" +
                       "Check-in Notes:\n" +
                       "  Code Reviewer:\n" +
                       "  Performance Reviewer:\n" +
                       "  Security Reviewer:\n" +
                       "\n" +
                       "-------------------------------------------------------------------------------\n" +
                       "Changeset: 110545\n" +
                       "User: s.otheruser\n" +
                       "Date: Thursday, May 07, 2009 1:03:27 PM\n" +
                       "\n" +
                       "Comment:\n" +
                       "  Get debug unit tests running again\n" +
                       "  build separate vc90.pdb files for each proj included in unit tests\n" +
                       "  RTF code stubs\n" +
                       "  Improved scaling for word fragments with spaces in them - required more accurate double char sizes rather than rounded int char sizes\n" +
                       "  Added unit tests for scaling probs that still exist\n" +
                       "  Added unit test to prove RegSetValue MSDN docs\n" +
                       "\n" +
                       "\n" +
                       "Items:\n" +
                       "  edit $/Project/Dev/Misc/MFCcharacterCaptureTest/MFCcharacterCaptureTest/MFCcharacterCaptureTest.vcproj\n" +
                       "  edit $/Project/Dev/Misc/MFCcharacterCaptureTest/MFCcharacterCaptureTest/MFCcharacterCaptureTestDlg.cpp\n" +
                       "  edit $/Project/Dev/Misc/MFCcharacterCaptureTest/MFCcharacterCaptureTest/characterMapTests.cpp\n" +
                       "  edit $/Project/Dev/Misc/MFCcharacterCaptureTest/MFCcharacterCaptureTest/ViewcharacterMapBinDlg.cpp\n" +
                       "  add  $/Project/Dev/Misc/MFCcharacterCaptureTest/MFCcharacterCaptureTest/WordExtraSpaces.bin\n" +
                       "  edit $/Project/Dev/ProjectUnitTests/CoreTests.cpp\n" +
                       "  edit $/Project/Dev/ProjectUnitTests/CoreTests.h\n" +
                       "  edit $/Project/Dev/ProjectUnitTests/characterCaptureTests.cpp\n" +
                       "  edit $/Project/Dev/projTxNT/SRC/APIHook.cpp\n" +
                       "  add  $/Project/Dev/projTxNT/SRC/Debug.cpp\n" +
                       "  add  $/Project/Dev/projTxNT/SRC/Debug.h\n" +
                       "  edit $/Project/Dev/projTxNT/SRC/RecordcharacterOut.cpp\n" +
                       "  edit $/Project/Dev/projTxNT/SRC/RecordcharacterOut.h\n" +
                       "  edit $/Project/Dev/projTxNT/SRC/projTxNT.cpp\n" +
                       "  edit $/Project/Dev/projTxNT/SRC/projTXNT.vcproj\n" +
                       "  edit $/Project/Dev/Src/DebugcharacterCapture.cpp\n" +
                       "  edit $/Project/Dev/Src/DebugcharacterCapture.h\n" +
                       "  edit $/Project/Dev/Src/IcharacterMap.h\n" +
                       "  edit $/Project/Dev/Src/Project.vcproj\n" +
                       "  edit $/Project/Dev/Src/characterCapture.cpp\n" +
                       "  add  $/Project/Dev/Src/characterCaptureHookCommon.h\n" +
                       "  edit $/Project/Dev/Src/characterCaptureStrategyWin32.cpp\n" +
                       "  edit $/Project/Dev/Src/characterCaptureStrategyWin32.h\n" +
                       "  edit $/Project/Dev/Src/characterMap.cpp\n" +
                       "  edit $/Project/Dev/Src/characterMap.h\n" +
                       "  edit $/Project/Dev/Src/characterScalingStrategies.cpp\n" +
                       "  edit $/Project/Dev/Src/characterScalingStrategies.h\n" +
                       "  edit $/Project/Project.sln\n" +
                       "\n" +
                       "Check-in Notes:\n" +
                       "  Code Reviewer:\n" +
                       "  Performance Reviewer:\n" +
                       "  Security Reviewer:\n" +
                       "\n" +
                       "-------------------------------------------------------------------------------\n" +
                       "Changeset: 110544\n" +
                       "User: s.otheruser\n" +
                       "Date: Thursday, May 07, 2009 12:58:38 PM\n" +
                       "\n" +
                       "Comment:\n" +
                       "    Remove Edit-and-Continue PDB files on some projs, we don't use that capability anyway and it just bloats the pdb files and causes extra build times - need to do it for all projs\n" +
                       "    Change output of vc90.pdb compiler pdb file to be {projname}vc90.pdb so that file for mult projs can exist in same dir, helping unit tests build \n" +
                       "\n" +
                       "Items:\n" +
                       "    edit $/Project/Dev/Add-ins/BrowserHelperObject/BHO.vcproj\n" +
                       "    edit $/Project/Dev/DataStore/DataStore.vcproj\n" +
                       "    edit $/Project/Dev/ExtensionArchitecture/ExtensionArchitecture.vcproj\n" +
                       "    edit $/Project/Dev/ProjectCommon/ProjectCommon.vcproj\n" +
                       "    edit $/Project/Dev/ProjectEditor/ProjectEditor.vcproj\n" +
                       "    edit $/Project/Dev/ProjectEditorRes/ProjectEditorRes.vcproj\n" +
                       "    edit $/Project/Dev/ProjectRes/ProjectRes.vcproj\n" +
                       "    edit $/Project/Dev/ProjectUnitTests/ProjectEditorUnitTestsEXE.vcproj\n" +
                       "    edit $/Project/Dev/ProjectUnitTests/ProjectUnitTestsEXE.vcproj\n" +
                       "    edit $/Project/Dev/projPriv/projPriv/projPriv.vcproj\n" +
                       "\n" +
                       "Check-in Notes:\n" +
                       "    Code Reviewer:\n" +
                       "    Performance Reviewer:\n" +
                       "    Security Reviewer:\n" +
                       "\n" +
                       "-------------------------------------------------------------------------------\n" +
                       "Changeset: 110531\n" +
                       "User: a.hutton\n" +
                       "Date: Thursday, May 07, 2009 10:59:21 AM\n" +
                       "\n" +
                       "Comment:\n" +
                       "   deleted config\n" +
                       "\n" +
                       "Items:\n" +
                       "    delete $/project/CruiseControl/ccnet.config\n" +
                       "\n" +
                       "Check-in Notes:\n" +
                       "    Code Reviewer:\n" +
                       "    Performance Reviewer:\n" +
                       "    Security Reviewer:\n" +
                       "\n" +
                       "-------------------------------------------------------------------------------\n" +
                       "Changeset: 110515\n" +
                       "User: a.anotheruser\n" +
                       "Date: Wednesday, May 06, 2009 5:13:35 PM\n" +
                       "\n" +
                       "Comment:\n" +
                       "    Updated the solution for the accessory sample applications to work with VS2008.\n" +
                       "\n" +
                       "Items:\n" +
                       "    edit $/Project/Dev/Extensions/Samples.sln\n" +
                       "    edit $/Project/Dev/Extensions/WPFOutput/WPFOutput.csproj\n" +
                       "\n" +
                       "Check-in Notes:\n" +
                       "    Code Reviewer:\n" +
                       "    Performance Reviewer:\n" +
                       "    Security Reviewer:\n" +
                       "\n" +
                       "-------------------------------------------------------------------------------\n" +
                       "Changeset: 110514\n" +
                       "User: s.user\n" +
                       "Date: Wednesday, May 06, 2009 5:12:32 PM\n" +
                       "\n" +
                       "Comment:\n" +
                       "    Refactor some window related functions into CWndUtil\n" +
                       "\n" +
                       "Items:\n" +
                       "    edit $/Project/Dev/Src/IMAGE.CPP\n" +
                       "    edit $/Project/Dev/Src/OldcharacterCapture.cpp\n" +
                       "    edit $/Project/Dev/Src/SCROLL.CPP\n" +
                       "    edit $/Project/Dev/Src/SELECT.CPP\n" +
                       "    delete $/Project/Dev/Src/SELECT.H\n" +
                       "    edit $/Project/Dev/Src/TopSecret.cpp\n" +
                       "    edit $/Project/Dev/Src/TopSecret.h\n" +
                       "    edit $/Project/Dev/Src/characterCapture.cpp\n" +
                       "    edit $/Project/Dev/Src/VIDEO.CPP\n" +
                       "    edit $/Project/Dev/Src/WndUtil.cpp\n" +
                       "    edit $/Project/Dev/Src/WndUtil.h\n" +
                       "\n" +
                       "Check-in Notes:\n" +
                       "    Code Reviewer:\n" +
                       "    Performance Reviewer:\n" +
                       "    Security Reviewer:\n" +
                       "\n" +
                       "-------------------------------------------------------------------------------\n" +
                       "Changeset: 110511\n" +
                       "User: s.user\n" +
                       "Date: Wednesday, May 06, 2009 4:37:56 PM\n" +
                       "\n" +
                       "Comment:\n" +
                       "    Add extra hardware checks for entering Direct3D mode.\n" +
                       "\n" +
                       "Items:\n" +
                       "    edit $/Project/Dev/Src/TopSecret.cpp\n" +
                       "\n" +
                       "Check-in Notes:\n" +
                       "    Code Reviewer:\n" +
                       "    Performance Reviewer:\n" +
                       "    Security Reviewer:\n" +
                       "\n" +
                       "-------------------------------------------------------------------------------\n" +
                       "Changeset: 110495\n" +
                       "User: a.user\n" +
                       "Date: Wednesday, May 06, 2009 2:16:26 PM\n" +
                       "\n" +
                       "Comment:\n" +
                       "    Japanese accessory help files from Matt\n" +
                       "\n" +
                       "Items:\n" +
                       "    add $/Project/_Branches/branch1/Project/Dev/Extensions/MSExcel//ProjectExcelOutput.chm\n" +
                       "    add $/Project/_Branches/branch1/Project/Dev/Extensions/MSPowerPoint/ProjectPowerPointOutput.chm\n" +
                       "    add $/Project/_Branches/branch1/Project/Dev/Extensions/MSWord//ProjectWordOutput.chm\n" +
                       "\n" +
                       "Check-in Notes:\n" +
                       "    Code Reviewer:\n" +
                       "    Performance Reviewer:\n" +
                       "    Security Reviewer:\n" +
                       "\n";

        string oneEntry = "-------------------------------------------------------------------------------\n" +
                          "Changeset: 996969\n" +
                          "User: s.user\n" +
                          "Date: Thursday, May 14, 2009 5:47:53 PM\n" +
                          "\n" +
                          "Comment:\n" +
                          "  Delete unused directory\n" +
                          "\n" +
                          "Items:\n" +
                          "  delete $/maintrunk/Dev/somefolder/folder;X13366\n" +
                          "\n" +
                          "Check-in Notes:\n" +
                          "  Code Reviewer:\n" +
                          "  Performance Reviewer:\n" +
                          "  Security Reviewer:\n\n";
       

        DateTime oldestEntry = DateTime.Parse("2009-05-02T02:48:50Z");
        DateTime newestEntry = DateTime.Parse("2009-05-16T23:09:45Z");

        private VstsHistoryParser vsts = new VstsHistoryParser();

        [Test]
        public void ParsingEmptyLogProducesNoModifications()
        {
            Modification[] modifications = vsts.Parse(new StringReader(emptyLog), oldestEntry, newestEntry);
            Assert.AreEqual(0, modifications.Length);
        }

        [Test]
        public void ParsingSingleLogMessageProducesOneModification()
        {
            Modification[] modifications = vsts.Parse(new StringReader(oneEntry), oldestEntry, newestEntry);

            Assert.AreEqual(1, modifications.Length);

            Modification expected = modifications[0];
            expected.Type = "delete";
            expected.FileName = string.Empty; ;
            expected.FolderName = @"$/maintrunk/Dev/somefolder/folder";
            expected.ModifiedTime = CreateDate("2009-05-14T17:47:53Z");
            expected.ChangeNumber = "996969";
            expected.UserName = "s.user";
            expected.Comment = "Delete unused directory";

            Assert.AreEqual(expected, modifications[0]);
        }

        [Test]
        public void ParsingLotsOfEntries()
        {
            Modification[] modifications = vsts.Parse(new StringReader(fullLog), oldestEntry, newestEntry);

            Assert.AreEqual(59, modifications.Length);

            Modification mbrMod1 = new Modification();
            mbrMod1.Type = "edit";
            mbrMod1.FileName = "WndUtil.cpp";
            mbrMod1.FolderName = "$/Project/Dev/Src";
            mbrMod1.ModifiedTime = CreateDate("2009-05-07T14:31:10");
            mbrMod1.ChangeNumber = "110558";
            mbrMod1.UserName = "s.user";
            mbrMod1.Comment = "Fix GDI leak";

            Assert.AreEqual(mbrMod1, modifications[0]);

            mbrMod1.Type = "delete";
            mbrMod1.FileName = "characterMapTests.cpp";
            mbrMod1.FolderName = "$/Project/Dev/Misc/MFCcharacterCaptureTest/MFCcharacterCaptureTest";
            mbrMod1.ModifiedTime = CreateDate("2009-05-07T13:47:46");
            mbrMod1.ChangeNumber = "110551";
            mbrMod1.UserName = "s.otheruser";
            mbrMod1.Comment = "Cleanup tests a little";

            Assert.AreEqual(mbrMod1, modifications[1]);                   
        }

        [Test]
        public void TwoEntriesOutsideOfRequestedTimeRangeAreIgnored()
        {
            Modification[] modifications = vsts.Parse(new StringReader(fullLogWithTooOld), oldestEntry, newestEntry);
            Assert.AreEqual(57, modifications.Length);
        }
       
        [Test]
        public void ParseModificationWithAddAction()
        {
            Modification[] mods = vsts.Parse(new StringReader(singleAddLog), oldestEntry, newestEntry);
            Assert.AreEqual("add", mods[0].Type);
        }

        [Test]
        public void ParseModificationWithEditAction()
        {
            Modification[] mods = vsts.Parse(new StringReader(singleEditLog), oldestEntry, newestEntry);
            Assert.AreEqual("edit", mods[0].Type);
        }

        [Test]
        public void ParseModificationWithDeleteAction()
        {
            Modification[] mods = vsts.Parse(new StringReader(singleRemoveLog), oldestEntry, newestEntry);
            Assert.AreEqual("delete", mods[0].Type);
        }

        private DateTime CreateDate(string dateString)
        {
            return DateTime.Parse(dateString);
        }
    }
}