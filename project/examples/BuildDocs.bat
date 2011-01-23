@echo off

REM *******************************************************
REM ** 1. CHECK OUT THE DOCS FROM SOURCE CONTROL & BUILD **
REM *******************************************************

	REM *********************
	REM ***** LOW LEVEL *****
	REM *********************
	p4 edit "//Libraries/Lowlevel/Documentation/xxxx Low Level.chm"
	doxygen "X:\Workflow\Doxygen\xxxx_ll"


	REM ****************
	REM ***** MAIN *****
	REM ****************
	p4 edit "//Libraries/Midlevel/Main/Documentation/xxxx Mid Level Main.chm"
	doxygen "X:\Workflow\Doxygen\xxxx_ml_main"


	REM ************************
	REM ***** SCENE MANAGER*****
	REM ************************
	p4 edit "//Libraries/Midlevel/SceneManager/Documentation/xxxx Mid Level Scene Manager.chm"
	doxygen "X:\Workflow\Doxygen\xxxx_ml_scenemanager"


	REM ****************
	REM ***** WATER*****
	REM ****************
	p4 edit "//Libraries/MidLevel/Water/Documentation/xxxx Mid Level Water.chm"
	doxygen "X:\Workflow\Doxygen\xxxx_ml_water"


	REM ***********************
	REM ***** RIGID BODIES*****
	REM ***********************
	p4 edit "//Libraries/Midlevel/RigidBodies/Documentation/xxxx Mid Level Rigid Bodies.chm"
	doxygen "X:\Workflow\Doxygen\xxxx_ml_rigidbodies"



REM ************************************************
REM ** 2. CHECK THE DOCS BACK INTO SOURCE CONTROL **
REM ************************************************

p4 change -o > xx.tmp
p4 submit -i < "X:\Workflow\Doxygen\changelist.tmp"
