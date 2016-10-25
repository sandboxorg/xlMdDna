﻿namespace xlMdDna {

	using ExcelDna.Integration;
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Windows.Forms;

	public static class xlMermaid {
		private static DirectoryInfo MyDoc { get { return new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)); } }
		private static dynamic xl = ExcelDnaUtil.Application;
		private static bool initEnd = false;
		private static Form wind;
		private static WebBrowser web;
		private static int width = 400 * 2;
		private static int height = 400 * 2;
		private static bool firstTime = true;
		private static bool secondTime = true;
		private static dynamic lastStyle = "zoom:300%;";
		private static dynamic lastPos = null;
		private static string md;

		private static string defhtml = @"
<!DOCTYPE html>
<html lang='ja'>
<head>
<meta charset='utf-8'>
<meta http-equiv='X-UA-Compatible' content='IE=edge,chrome=1'>
<meta name='viewport' content='width=device-width, initial-scale=1'>
<script src='https://cdnjs.cloudflare.com/ajax/libs/mermaid/6.0.0/mermaid.min.js'></script>
<link rel='stylesheet' type='text/css' href='https://cdnjs.cloudflare.com/ajax/libs/mermaid/6.0.0/mermaid.min.css'>
</head>
<body style='background-color: #ffffff;'>
<div id='preview' class='mermaid'>
{MMSTR}
</div>
<script>
mermaid.initialize({flowchart:{htmlLabels:false}})
</script>
</body>
</html>
";

		[ExcelFunction(Name = "Mermaid", Description = "About xlMdDna")]
		public static string Mermaid(dynamic[,] args) {
			initEnd = init();

			var buf = getArgsString(args);
			md = string.Join("\n", buf).Replace("\u00A0", " ");
			try {
				getPreviewWindow(md);
			}
			catch (Exception ex) {
				Clipboard.SetText($"Err: mermaidFail\n{ex.Message}");
			}

			return "OK";
		}

		private static bool init() {
			if (!initEnd) {
				wind = new Form();
				wind.Width = width;
				wind.Height = height;
				wind.AutoScaleMode = AutoScaleMode.Font;
				//wind.AutoSize = true;
				wind.BackColor = Color.White;
				wind.TopMost = true;
				web = new WebBrowser();
				web.Visible = true;
				web.Dock = DockStyle.Fill;
				wind.Controls.Add(web);
				wind.Show();

				//wind.SizeChanged += (s, e) => { firstTime = false; };
				//wind.Move += (s, e) => { firstTime = false; };
				//web.ClientSizeChanged += (s, e) => { firstTime = false; };
				//web.SizeChanged += (s, e) => { firstTime = false; };
				//web.DocumentCompleted += (s, e) => {
				//	if (firstTime) {
				//		firstTime = false;
				//		var x = (int)(web.Document.Window.Size.Width / 2 * 1.5);
				//		var y = 0;// (int)(web.Document.Window.Size.Height/2*.5);
				//		web.Document.Body.Style = "zoom:300%;";
				//		web.Document.Window.ScrollTo(x, y);
				//		getPreviewWindow(md);
				//	}else if (secondTime) {
				//		secondTime = false;
				//		var x = (int)(web.Document.Window.Size.Width / 2 * 1.5);
				//		var y = 0;// (int)(web.Document.Window.Size.Height/2*.5);
				//		web.Document.Body.Style = "zoom:300%;";
				//		web.Document.Window.ScrollTo(x, y);
				//		getPreviewWindow(md);
				//	}
				//};

				wind.Closing += (s, e) => {
					lastStyle = web.Document.Body.Style;
					windCapture();
					e.Cancel = true;
					wind.Hide();
				};
			}
			if (!wind.Visible) {
				wind.Show();
			}
			wind.FormBorderStyle = FormBorderStyle.Sizable;
			web.ScrollBarsEnabled = true;

			return true;
		}

		private static IEnumerable<string> getArgsString(object[,] args) {
			var yLen = args.GetLength(0);
			var xLen = args.GetLength(1);
			var line = "";
			var str = "";
			var rgx = new Regex(@"^(\(|\[|\{)");
			for (var y = 0; y < yLen; y++) {
				line = "";
				for (var x = 0; x < xLen; x++) {
					try {
						if ((str = args[y, x].ToString()) != "ExcelDna.Integration.ExcelEmpty")
							line += (rgx.IsMatch(str) ? "" : " ") + str;
					}
					catch (Exception ex) {
						Clipboard.SetText($"Err: ReadCellFail\n{ex.Message}\n{args[y, x]}");
					}
				}
				yield return line;
			}
		}

		private static string sjisToUtf(string sjisstr) {
			Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
			//string sjisstr = sjisEnc.GetString(loaddata);
			byte[] bytesData = System.Text.Encoding.UTF8.GetBytes(sjisstr);
			Encoding utf8Enc = Encoding.GetEncoding("UTF-8");
			return utf8Enc.GetString(bytesData);
		}

		private static void windCapture(string fileName = "preview.png") {
			try {
				wind.FormBorderStyle = FormBorderStyle.None;
				web.ScrollBarsEnabled = false;
				lastPos = web.Document.Window.Position;
				//wind.Activate();
				saveSvg();
				SendKeys.SendWait("%{PRTSC}");
				var bmp = (Bitmap)Clipboard.GetImage();
				bmp.MakeTransparent(Color.White);
				//Clipboard.SetImage(bmp);
				var path = $"{MyDoc.FullName}/{fileName}";
				bmp.Save(path);
				//xl.ActiveSheet.Paste();
				xl.ActiveSheet.Shapes.AddPicture(path, 0, -1, 0, 0, bmp.Width, bmp.Height);
			}
			catch (Exception ex) {
				MessageBox.Show($"Err: {ex.Message}");
			}
		}

		private static void saveSvg(string fileName = "preview.svg") {
			try {
				var pv = web.Document.GetElementById("preview");
				var svgStr = pv.InnerHtml;
				Clipboard.SetText(svgStr);
				File.WriteAllText($"{MyDoc.FullName}/{fileName}", svgStr);
			}
			catch (Exception) { }
		}

		private static Form getPreviewWindow(string md) {
			web.DocumentText =
				defhtml
					.Replace("{MMSTR}", md)
			//.Replace("{MMCSS}", mmcss)
			//.Replace("{MMJS}", mmjs)
			;
			return wind;
		}
	}
}