/*
 * cm17a.c
 *
 * Simple command line utility to talk to a cm17a X-10 controller 
 * through win32 serial port commands.
 * 
 *  This code is (c)2000, Adam Briggs. All Rights Reserved.
 *  
 *  Permission to use, copy, and distribute this software and its
 *  documentation for any purpose and without fee is hereby granted, provided
 *  that the above copyright notice appear in all copies and that both that
 *  copyright notice and this permission notice appear in supporting
 *  documentation, and that the name Adam Briggs not be used in
 *  advertising or publicity pertaining to distribution of the software
 *  without specific, written prior permission.
 *  
 *                              *** DISCLAIMER ***
 *   
 *  ADAM BRIGGS DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE,
 *  INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS, IN NO
 *  EVENT SHALL ADAM BRIGGS BE LIABLE FOR ANY SPECIAL, INDIRECT OR
 *  CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF
 *  USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR
 *  OTHER TORTUOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
 *  PERFORMANCE OF THIS SOFTWARE.
 *  
 *                                     ***
 */

#include <windows.h>
#include <stdio.h>

#define DELAY  35      /* delay between bits */
#define HEADER 0xd5aa  /* magic command header */
#define FOOTER 0xad    /* magic command footer */

#define ON     0x0000  /* magic code for on */
#define OFF    0x0020  /* magic code for off */
#define BRIGHT 0x00a8  /* magic code for bright */
#define DIM    0x00b8  /* magic code for dim */

#define INVALIDCOMMAND 0xffff

const unsigned long dwHouseCode[] =
{
  0x6000, /* a */
  0x7000, /* b */
  0x4000, /* c */
  0x5000, /* d */
  0x8000, /* e */
  0x9000, /* f */
  0xa000, /* g */
  0xb000, /* h */
  0xe000, /* i */
  0xf000, /* j */
  0xc000, /* k */
  0xd000, /* l */
  0x0000, /* m */
  0x1000, /* n */
  0x2000, /* o */
  0x3000  /* p */
} ;

const int maxHouseCode = sizeof(dwHouseCode) / sizeof(dwHouseCode[0]) ;

const unsigned long dwUnitCode[] =
{
  0x0000, /* 1 */
  0x0010, /* 2 */
  0x0008, /* 3 */
  0x0018, /* 4 */
  0x0040, /* 5 */
  0x0050, /* 6 */
  0x0048, /* 7 */
  0x0058, /* 8 */
  0x0400, /* 9 */
  0x0410, /* 10 */
  0x0408, /* 11 */
  0x0400, /* 12 */
  0x0440, /* 13 */
  0x0450, /* 14 */
  0x0448, /* 15 */
  0x0458  /* 16 */
} ;

const int maxUnitCode = sizeof(dwUnitCode) / sizeof(dwUnitCode[0]) ;


HANDLE   thePort ;      /* handle to the serial port */


/*
 * Twiddle a byte of raw data out to the cm17a device
 */
void Cm17aSendByte(unsigned char theByte)
{
  int theMask = 0x80 ;

  while (theMask)
  {
    if (theByte & theMask)
    {
      EscapeCommFunction(thePort, SETRTS) ;
      EscapeCommFunction(thePort, CLRDTR) ;      
    }
    else
    {
      EscapeCommFunction(thePort, SETDTR) ;
      EscapeCommFunction(thePort, CLRRTS) ;
    }
    Sleep(DELAY) ;

    EscapeCommFunction(thePort, SETDTR) ;
    EscapeCommFunction(thePort, SETRTS) ;
    Sleep(DELAY) ;

    theMask >>= 1 ;
  }
}

/*
 * Twiddle a dword of raw data out to the cm17a device
 */
void Cm17aSendDword(unsigned long theDword)
{
  unsigned long theMask = 0x8000 ;

  while (theMask)
  {
    if (theDword & theMask)
    {
      EscapeCommFunction(thePort, SETRTS) ;
      EscapeCommFunction(thePort, CLRDTR) ;      
    }
    else
    {
      EscapeCommFunction(thePort, SETDTR) ;
      EscapeCommFunction(thePort, CLRRTS) ;
    }
    Sleep(DELAY) ;

    EscapeCommFunction(thePort, SETDTR) ;
    EscapeCommFunction(thePort, SETRTS) ;
    Sleep(DELAY) ;

    theMask >>= 1 ;
  }
}

/*
 * Open the given com port and reset the attached cm17a
 */
int Cm17aStart(int comPortNumber)
{
  char comPortString[80] ;

  sprintf(comPortString, "\\\\.\\COM%d", comPortNumber) ;

  thePort = CreateFile(comPortString, GENERIC_READ | GENERIC_WRITE,
      0, NULL, OPEN_EXISTING, FILE_FLAG_WRITE_THROUGH, NULL) ;

  if (thePort != INVALID_HANDLE_VALUE)
  {
    /* Reset the device */
    EscapeCommFunction(thePort, CLRDTR) ;
    EscapeCommFunction(thePort, CLRRTS) ;
    Sleep(DELAY) ;
    EscapeCommFunction(thePort, SETDTR) ;
    EscapeCommFunction(thePort, SETRTS) ;
    Sleep(DELAY) ;

    return 1 ;
  }
  else
    return 0 ;
}


/*
 * Close the com port
 */
int Cm17aStop(void)
{ 
  CloseHandle(thePort) ;

  return 1 ;
}


/*
 * Given a command string, return the magic function code
 * to be sent to the cm17a device. If the command string
 * is invalid, return INVALIDCOMMAND.
 *
 * Commands consist of a house code followed by either
 * a unit code and an on/off command or a bright or dim
 * command to be sent to the last unit code that has
 * been turned on within the given house code.
 */
unsigned long Cm17aMakeCommand(char *theCommandString)
{
  unsigned long theCommand ;
  int           houseCodeIndex ;
  int           unitCodeIndex ;
  int           functionIndex ;

  theCommand = 0 ;

  houseCodeIndex = (int)toupper(theCommandString[0]) - 65 ;

  if ((houseCodeIndex < maxHouseCode) &&
      (houseCodeIndex >= 0))
  {
    theCommand |= dwHouseCode[houseCodeIndex] ;

    unitCodeIndex = atol(&theCommandString[1]) - 1 ;

    if ((unitCodeIndex < maxUnitCode) &&
        (unitCodeIndex >= 0))
    {
      theCommand |= dwUnitCode[unitCodeIndex] ;

      functionIndex = strlen(theCommandString) - 3 ;

      if ((functionIndex > 0) &&
          (stricmp(&theCommandString[functionIndex], "OFF") == 0))
        theCommand |= OFF ;
    }
    else
    {
      if (stricmp(&theCommandString[1], "DIM") == 0)
        theCommand |= DIM ;
      else
        if (stricmp(&theCommandString[1], "BRIGHT") == 0)
          theCommand |= BRIGHT ;
        else
          theCommand = INVALIDCOMMAND ;
    }

  }
  else
    theCommand = INVALIDCOMMAND ;

  return theCommand ;
}

/*
 * This is the real top-level 'Do It' command that
 * takes a com port and an ascii command and does
 * everything needed to send the data out to the
 * cm17a device.
 */
int Cm17aSendCommand(int thePort, char *theCommand)
{
  int           ok = 0 ;
  unsigned long command ;

  command = Cm17aMakeCommand(theCommand) ;

  if (command == INVALIDCOMMAND)
    ok = INVALIDCOMMAND ;
  else
    if (Cm17aStart(thePort))
    {
      Cm17aSendDword(HEADER) ;
      Cm17aSendDword(command) ;
      Cm17aSendByte(FOOTER) ;
      Cm17aStop() ;
      ok = 1 ;
    }
  return ok ;
}



int main(int argc, char* argv[])
{
  int theArg = 2 ;
  int ok     = 1 ;

  if (argc < 3)
  {
    printf("usage: cm17a [COM] [command] <command> <...>\n") ;
    printf("\n       [COM] = com port of cm17a device\n") ;
    printf("\n       [command] = command(s) to send (ie A1ON B2OFF ADIM ABRIGHT)\n\n") ;
  }
  else
    while (argv[theArg] &&
           ok)
    {
      ok = Cm17aSendCommand(atol(argv[1]), argv[theArg]) ;

      if (ok == INVALIDCOMMAND)
      {
        printf("%s is an invalid command.\n", argv[theArg]) ;
        ok = 1 ;
      }

      theArg++ ;
    }

  if (!ok)
    printf("Couldn't open COM %d\n", atol(argv[1])) ;

  return 1 ;
}
