﻿/*
 
The MIT License (MIT)

Copyright (c) 2016 WaTo.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

namespace xlMdDna {

	using ExcelDna.Integration;
	using System;
	using System.Windows.Forms;

	public class App {
		private static Version AppVer = new Version(0, 2, 0);

		[ExcelCommand(MenuName = "xlMdDna", MenuText = "About")]
		public static void About() {
			MessageBox.Show( $@"xlMdDna.	Ver: {AppVer}
thanks
  Excel-DNA: https://excel-dna.net/
  mermaid: https://knsv.github.io/mermaid
");
		}
	}
}