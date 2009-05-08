 Please read this annoying legal crap:

 This program is (c)2000, Adam Briggs. All Rights Reserved.
 
 Permission to use, copy, and distribute this software and its
 documentation for any purpose and without fee is hereby granted, provided
 that the above copyright notice appear in all copies and that both that
 copyright notice and this permission notice appear in supporting
 documentation, and that the name Adam Briggs not be used in
 advertising or publicity pertaining to distribution of the software
 without specific, written prior permission.
 
                             *** DISCLAIMER ***
  
 ADAM BRIGGS DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE,
 INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS, IN NO
 EVENT SHALL ADAM BRIGGS BE LIABLE FOR ANY SPECIAL, INDIRECT OR
 CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF
 USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR
 OTHER TORTUOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
 PERFORMANCE OF THIS SOFTWARE.
 
                                    ***
 
 That being said,

 Here is the scoop:
 ==================

 IMHO, all of the worthwhile home automation control software runs
 on Linux (and don't get me wrong, I think Linux rocks the house)
 but us poor shmucks who are addicted to certain win32 based video
 games are nearly unable to make our lamps flash on and off at
 the inconvenient intervals we so often desire.  So, this program is
 for those of you who want to make nifty home control setups but are
 too lazy to write the software to do it.

 The included source file should compile with msdev4->6 without any
 trouble and if it doesn't then just be thankful that I've included
 and executable because that's all the help you're going to get from
 me.


 Here's how to use it:
 =====================

 To use the executable, just run it from the command line and give it
 the com port of your cm17a dongle and the string of commands that you
 want it to send.

 For example, my cm17a is hooked up to com 2 and I want to turn on
 the two lights I have on house code A. To do this I type:

 cm17a 2 a1on a2on


 To turn the lights off again I type:

 cm17a 2 a1off a2off


 The dim and bright commands are also supported:

 cm17a 2 a2on adim adim adim

 cm17a 2 abright abright


 According to the cm17a protocol reference each dim command dims by
 5% (though it looks more like 10% to me, and not quite linear).


 How to get in touch with me:
 ============================

 I don't mean to sound like the jerk that I am, but chances are
 that I probably don't want to hear from you. I am not going to
 provide you with technical support and I don't feel sorry for
 you if you or your loved ones used my program to activate the
 toaster that you had in the bathtub with you. I am not planning
 any enhancements or bug fixes to this program (though you are
 welcome to do so if you like) and as far as I am concerned this
 is a done deal.

 If you would like to contact me for some reason other than typical
 net badgery (like maybe to e-mail me something worthwhile that you've
 based on this program) you can contact me at atom_bomb@rocketmail.com

