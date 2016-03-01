using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ANDREICSLIB.Licensing;

namespace RegexHelper
{
    public partial class Form1 : Form
    {
        #region licensing
        private const String HelpString = "";

        private readonly String OtherText =
            @"©" + DateTime.Now.Year +
            @" Andrei Gec (http://www.andreigec.net)

Licensed under GNU LGPL (http://www.gnu.org/)

Zip Assets © SharpZipLib (http://www.sharpdevelop.net/OpenSource/SharpZipLib/)
";

        #endregion


        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Licensing.LicensingForm(this, menuStrip1, HelpString, OtherText);
            Run();
        }

        private string RegexSanatise(string raw)
        {
            var s = Regex.Escape(raw);
            return s;
        }

        private void Run()
        {
            var strs = patterns.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None).Select(RegexSanatise).ToList();
            AddAllCombinations(strs);
        }

        private void AddAllCombinations(List<string> strs)
        {
            var res = new List<string>();
            res.Add(Controller.Extract(strs));
            res.Add(Controller.Extract(strs, true));
            res = res.Distinct().ToList();
            outputmatch.Text = res.Aggregate("", (a, b) => a + Environment.NewLine + b).Trim();
        }

        private void patterns_TextChanged_1(object sender, EventArgs e)
        {
            Run();
        }
    }
}
