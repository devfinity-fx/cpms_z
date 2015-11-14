/******************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * Open Source Host:
 *		http://reogrid.codeplex.com/
 *
 * Official Site:
 *		http://www.unvell.com/ReoGrid
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * This software released under LGPLv3 license.
 * Author: Jing Lu <dujid0 at gmail.com>
 * 
 * Copyright (c) 2012-2014 unvell.com, all rights reserved.
 * 
 ******************************************************************************

Package Library

  Minimum - Only the Grid Control and core features support
  Editor - Grid Control, Core features, and GUI Editor support
  Full - Grid Control, Core features, GUI Editor and Script Execution

DLL Description & Reference Dependency 

  DLL Name                    Description                            Min  Editor  Full
  --------------------------------------------------------------------------------------
  unvell.ReoGrid.dll          ReoGrid Control & Core features        Yes  Yes     Yes
  unvell.ReoGrid.Editor.dll   Controls and forms for ReoGrid Editor       Yes     Yes
  unvell.ReoScript.dll        .NET script language engine                         Yes
  unvell.ReoScriptEditor.dll  Controls and forms for script editing               Yes
  Antlr3.Runtime.dll          ANTLR gammar tool runtime                           Yes
  FastColoredTextBox.dll      Powerful colored highlight text editor              Yes

Note:
  Since 0.8.5 both unvell.common.dll and unvell.UIControls.dll are not necessary.
  And all namespace changed to 'unvell' from 'Unvell'.

Usage

  Create instance for control and add it into any component, like Form or Panel.

    var grid = new Unvell.ReoGrid.ReoGridControl()
    {
        Dock = DockStyle.Fill,
    };

    this.Controls.Add(grid);


Development Information

 * 
 *   LeadHead                            Header
 *           \                             |
 *          +-\---+---------------+--------|-------+----------------+
 *          |     |        A      |        B       |        C       |
 *          +-----+---------------+----------------+----------------+
 *          |  1  |     Cell A1   |     Cell B1    |     Cell C1    |
 *          +-----+---------------+----------------+----------------+
 *          |  2  |     Cell A2   |     Cell B2    |     Cell C2    |
 *          +--|--+---------------+----------------+----------------+
 *             |
 *         Row Index
 * 
 **** Cell Data Struct ********************************************************
 * 
 *   Rows      up to 2^16   = 1048576
 *   Columns   up to 2^8    = 32768
 *   
 *   Implementation class: RegularTreeArray
 *                               
 *     ---             +----------------------+
 *      |              +    16x8 Page Index   +  
 *      |              +----------------------+
 *      |                   /          \
 *    depth:      +------------+      +------------+
 *      5         +  Sub Page  +      +  Sub Page  +
 *      |         +------------+      +------------+
 *      |           /                            \
 *      |        ...                              ...
 *      |       /    \                           /    \
 *     ---   ...      ...                      ...     ...
 *         /\           /\                    /\         /\
 *    Cells Data    Cells Data    ...     Cells Data    Cells Data
 *
 * 
 **** Formula & Script ********************************************************
 * 
 *   ReoGrid uses ReoScript to implement the formula evaluation and script 
 *   execution.
 *   
 *   ReoScript is ECMAScript-like .NET Script Language Engine, provided by
 *   unvell too, free and open source. More info available at:
 *   http://reoscript.codeplex.com
 *   
 *   Open the script editor from ReoGrid Editor, run the following script:
 *   
 *   function hello() {
 *     alert('hello world!');
 *   }
 *   
 *   hello();
 *   
 *   The message box shown and 'hello world!' will be printed out:
 *   
 *                  +-----------------------------------+
 *                  |                                   |
 *                  |  hello world!                     |
 *                  |                                   |
 *                  |                       +------+    |
 *                  |                       |  OK  |    |
 *                  |                       +------+    |
 *                  +-----------------------------------+
 *   
 *   ReoGrid provides the API can be used to access control in script.
 *   Global object 'grid' always point to the instance of current control.
 *   'grid' object provides many method like 'getCell' which can be used to
 *   retrieve cell with specified position, e.g.:
 *   
 *       grid.getCell(0, 0).data = 'hello world';
 *       
 *   The data of Cell[0,0] will be changed and 'hello world' will be printed
 *   out.
 *   
 *   Script expression can be used as formula in cell. The following
 *   expressions are available:
 *   
 *       =A1 + B2
 *       =A1 + (B2 + C3) * 2
 *       =A3=="yes"?"true":"false"
 * 
 *   since A1:C3 is not an valid expression in ReoScript, it will be converted
 *   into 'new Range(0, 1, 3, 3)' before expression execution.
 *
 *       =sun(A1:C3)      ->    =sun(new Range(0, 0, 3, 3))
 *       =avg(B3:B10)     ->    =avg(new Range(1, 2, 1, 7))
 * 
 *   Call customize function is also valid expression, 
 *   it can be used as formula:
 *   
 *       =hello()
 *       =hello(A1:C3)    ->    =hello(new Range(0, 0, 3, 3))
 *	     
 ****************************************************************************/
