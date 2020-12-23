using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;

namespace RsHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            calculateExp();

            time.onEveryFinish += onTimeEnd;
        }

        #region costCalculator
        private bool fieldsFull1
        {
            get
            {
                return (nameEntry1.Text.Length != 0 &&
                    costEntry1.Text.Length != 0 && multEntry1.Text.Length != 0);
            }
        }

        private bool fieldsFull2
        {
            get
            {
                return (nameEntry2.Text.Length != 0 &&
                    multEntry2.Text.Length != 0 && costEntry2.Text.Length != 0);
            }
        }

        private void itemSelected1(object sender, EventArgs e)
        {
            removeButton1.Enabled = (nameBox1.SelectedItems.Count != 0);
            checkReplace1();
        }

        private void itemSelected2(object sender, EventArgs e)
        {
            removeButton2.Enabled = (nameBox2.SelectedItems.Count != 0);
            checkReplace2();
        }

        private void checkCanAdd1(object sender, EventArgs e)
        {
            addButton1.Enabled = fieldsFull1;
            checkReplace1();
        }

        private void checkCanAdd2(object sender, EventArgs e)
        {
            button4.Enabled = fieldsFull2;
            checkReplace2();
        }


        private void checkReplace1()
        {
            replaceButton1.Enabled = (nameBox1.SelectedItems.Count != 0 &&
                fieldsFull1);
        }

        private void checkReplace2()
        {
            replaceButton2.Enabled = (nameBox2.SelectedItems.Count != 0 &&
                fieldsFull2);
        }

        private void removeFrom1(object sender, EventArgs e)
        {
            int selected = nameBox1.SelectedIndex;
            nameBox1.Items.RemoveAt(selected);
            costBox1.Items.RemoveAt(selected);
            multBox1.Items.RemoveAt(selected);
            updateProfit();
        }

        private void removeFrom2(object sender, EventArgs e)
        {
            int selected = nameBox2.SelectedIndex;
            nameBox2.Items.RemoveAt(selected);
            costBox2.Items.RemoveAt(selected);
            multBox2.Items.RemoveAt(selected);
            updateProfit();
        }

        private void addTo1(object sender, EventArgs e)
        {
            nameBox1.Items.Add(nameEntry1.Text);
            costBox1.Items.Add(numberAbbreviationHandler.getNumberEstimate(numberAbbreviationHandler.replaceAbbreviation(costEntry1.Text),10));
            multBox1.Items.Add(numberAbbreviationHandler.getNumberEstimate(numberAbbreviationHandler.replaceAbbreviation(multEntry1.Text), 10));
            updateProfit();
        }

        private void addTo2(object sender, EventArgs e)
        {
            nameBox2.Items.Add(nameEntry2.Text);
            costBox2.Items.Add(numberAbbreviationHandler.getNumberEstimate(numberAbbreviationHandler.replaceAbbreviation(costEntry2.Text), 10));
            multBox2.Items.Add(numberAbbreviationHandler.getNumberEstimate(numberAbbreviationHandler.replaceAbbreviation(multEntry2.Text), 10));
            updateProfit();
        }

        private void updateProfit()
        {
            /*if (nameBox1.Items.Count == 0 || nameBox2.Items.Count == 0)
            {
                label8.Text = "0";
                label8.ForeColor = Color.Black;
                return;
            }*/

            int profit = 0;
            for(int i=0; i<nameBox1.Items.Count; i++)
            {
                profit -= (int) (numberAbbreviationHandler.replaceAbbreviation(costBox1.Items[i].ToString()) * numberAbbreviationHandler.replaceAbbreviation(multBox1.Items[i].ToString()));
            }

            for (int i = 0; i < nameBox2.Items.Count; i++)
            {
                profit += (int)(numberAbbreviationHandler.replaceAbbreviation(costBox2.Items[i].ToString()) * numberAbbreviationHandler.replaceAbbreviation(multBox2.Items[i].ToString()));
            }
            label8.Text = numberAbbreviationHandler.getNumberEstimate(profit, 3);
            label8.ForeColor = (profit >= 0) ? Color.Green : Color.Red;
        }

        private void replaceSelection1(object sender, EventArgs e)
        {
            nameBox1.Items[nameBox1.SelectedIndex] = nameEntry1.Text;
            costBox1.Items[nameBox1.SelectedIndex] = numberAbbreviationHandler.getNumberEstimate(numberAbbreviationHandler.replaceAbbreviation(costEntry1.Text), 10);
            multBox1.Items[nameBox1.SelectedIndex] = numberAbbreviationHandler.getNumberEstimate(numberAbbreviationHandler.replaceAbbreviation(multEntry1.Text), 10);
            updateProfit();
        }

        private void replaceSelection2(object sender, EventArgs e)
        {
            nameBox2.Items[nameBox2.SelectedIndex] = nameEntry2.Text;
            costBox2.Items[nameBox2.SelectedIndex] = numberAbbreviationHandler.getNumberEstimate(numberAbbreviationHandler.replaceAbbreviation(costEntry2.Text), 10);
            multBox2.Items[nameBox2.SelectedIndex] = numberAbbreviationHandler.getNumberEstimate(numberAbbreviationHandler.replaceAbbreviation(multEntry2.Text), 10);
            updateProfit();
        }

        private void copySelection1(object sender, EventArgs e)
        {
            if (nameBox1.SelectedItems.Count == 0)
                return;
            nameEntry1.Text = nameBox1.Items[nameBox1.SelectedIndex].ToString();
            costEntry1.Text = costBox1.Items[nameBox1.SelectedIndex].ToString();
            multEntry1.Text = multBox1.Items[nameBox1.SelectedIndex].ToString();
        }

        private void copySelection2(object sender, EventArgs e)
        {
            if (nameBox2.SelectedItems.Count == 0)
                return;
            nameEntry2.Text = nameBox2.Items[nameBox2.SelectedIndex].ToString();
            costEntry2.Text = costBox2.Items[nameBox2.SelectedIndex].ToString();
            multEntry2.Text = multBox2.Items[nameBox2.SelectedIndex].ToString();
        }

        private void checkNumbersCostCalc(object sender, KeyEventArgs e)
        {
            checkNumbers(sender, null);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            nameBox1.Items.Clear();
            costBox1.Items.Clear();
            multBox1.Items.Clear();

            nameBox2.Items.Clear();
            costBox2.Items.Clear();
            multBox2.Items.Clear();

            updateProfit();
        }

        private void saveState(object sender, EventArgs e)
        {
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName.Length == 0)
                return;

            StreamWriter sw = new StreamWriter(saveFileDialog1.OpenFile());

            for(int i=0; i<nameBox1.Items.Count; i++)
            {
                sw.WriteLine(nameBox1.Items[i].ToString() + ":" + costBox1.Items[i].ToString() + ":" + multBox1.Items[i].ToString());
            }

            sw.WriteLine("break");

            for (int i = 0; i < nameBox2.Items.Count; i++)
            {
                sw.WriteLine(nameBox2.Items[i].ToString() + ":" + costBox2.Items[i].ToString() + ":" + multBox2.Items[i].ToString());
            }
            sw.WriteLine("end");

            sw.Flush();
            sw.Close();
        }

        private void loadState(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName.Length == 0)
                return;

            nameBox1.Items.Clear();
            costBox1.Items.Clear();
            multBox1.Items.Clear();

            nameBox2.Items.Clear();
            costBox2.Items.Clear();
            multBox2.Items.Clear();

            StreamReader sr = new StreamReader(openFileDialog1.OpenFile());
            String[] items;
            while (true)
            {
                string line = sr.ReadLine();
                if (line == null || line == "break")
                    break;
                items = line.Split(':');
                nameBox1.Items.Add(items[0]);
                costBox1.Items.Add(items[1]);
                multBox1.Items.Add(items[2]);
            }

            while (true)
            {
                string line = sr.ReadLine();
                if (line == null || line == "end")
                    break;
                items = line.Split(':');
                nameBox2.Items.Add(items[0]);
                costBox2.Items.Add(items[1]);
                multBox2.Items.Add(items[2]);
            }

            sr.Close();
            updateProfit();
        }
        #endregion

        #region expCalculator
        int[] exp;

        private void calculateExp()
        {
            int points = 0;
            exp = new int[99];
            exp[0] = 0;
            for(int level=1; level<99; level++)
            {
                points += (int)Math.Floor(level + 300 * Math.Pow(2, (double)Decimal.Divide(level, 7)));
                exp[level] = (int)Math.Floor(Decimal.Divide(points, 4));
            }
        }

        private void checkAdd(object sender, EventArgs e)
        {
            addButtonExp.Enabled = nameEntryExp.Text.Length != 0 && expEntry.Value != 0;
        }

        int remainingExperience = 0;
        private void recalculateRemainingExp(object sender, EventArgs e)
        {
            if (levelEntry.Value == 0)
                return;

            int expToUse = (int)currentExpEntry.Value;
            if (currentLevelEntry.Value > 0 && exp[(int)(currentLevelEntry.Value-1)] > currentExpEntry.Value)
                expToUse = exp[(int)(currentLevelEntry.Value - 1)];

            int difference = exp[(int)(levelEntry.Value - 1)] - expToUse;
            if (difference > 0)
            {
                remainingExperience = difference;
                expLeft.Text = numberAbbreviationHandler.getNumberEstimate(difference, 3);
            }

            updateExpActions();
        }

        private void checkRemove(object sender, EventArgs e)
        {
            removeButtonExp.Enabled = nameList.SelectedItems.Count != 0;
        }

        private void addAction(object sender, EventArgs e)
        {
            nameList.Items.Add(nameEntryExp.Text);
            expList.Items.Add(expEntry.Value);

            updateExpActions();
        }

        private void removeButtonExp_Click(object sender, EventArgs e)
        {
            int index = nameList.SelectedIndex;
            nameList.Items.RemoveAt(index);
            expList.Items.RemoveAt(index);

            updateExpActions();
        }

        private void updateExpActions()
        {
            if(expList.Items.Count == 0)
                return;

            int total = 0;
            for(int i=0; i<expList.Items.Count; i++)
            {
                total += Int32.Parse(expList.Items[0].ToString());
            }
            actionsLeft.Text = ""+(int)Math.Round(Decimal.Divide((decimal)remainingExperience, total));
        }

        private void save(object sender, EventArgs e)
        {
            if (nameList.Items.Count == 0)
                return;

            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName.Length == 0)
                return;

            StreamWriter sw = new StreamWriter(saveFileDialog1.OpenFile());

            sw.WriteLine(currentExpEntry.Value + ":" + currentLevelEntry.Value + ":" + levelEntry.Value);
            for (int i = 0; i < nameList.Items.Count; i++)
            {
                sw.WriteLine(nameList.Items[i] + ":" + expList.Items[0]);
            }
            sw.WriteLine("end");

            sw.Flush();
            sw.Close();
        }

        private void load(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName.Length == 0)
                return;

            StreamReader sr = new StreamReader(openFileDialog1.OpenFile());

            nameList.Items.Clear();
            expList.Items.Clear();
            string[] expInfo = sr.ReadLine().Split(':');
            currentExpEntry.Value = Int32.Parse(expInfo[0]);
            currentLevelEntry.Value = Int32.Parse(expInfo[1]);
            levelEntry.Value = Int32.Parse(expInfo[2]);
            while (true)
            {
                string line = sr.ReadLine();
                if (line.Equals("end") || line == null)
                    break;

                string[] items = line.Split(':');

                nameList.Items.Add(items[0]);
                expList.Items.Add(items[1]);
            }

            sr.Close();
            recalculateRemainingExp(null, null);
            updateExpActions();
        }
        #endregion

        #region calculator
        private void calculate(object sender, EventArgs e)
        {
            evalEntry.Text = evalEntry.Text.ToLower();
            evalEntry.Text = numberAbbreviationHandler.replaceAbbreviations(evalEntry.Text);
            
            DataTable dt = new DataTable();
            evalEntry.Text = numberAbbreviationHandler.getNumberEstimate(Decimal.Parse(dt.Compute(evalEntry.Text, "").ToString()), 10);

        }

        private void evalEntry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                calculate(e, null);
        }
        
        Dictionary<String, String> lastSafeValues = new Dictionary<String, String>();
        private void checkNumbers(object sender, EventArgs e)
        {
            TextBox theSender = (TextBox)sender;
            if (!lastSafeValues.ContainsKey(theSender.Name))
                lastSafeValues.Add(theSender.Name, "");
            if (!Regex.IsMatch(theSender.Text, "^[\\d\\(\\)/*+-.kmbKMB]*$"))
            {
                theSender.Text = lastSafeValues[theSender.Name];
                theSender.SelectionStart = theSender.Text.Length;
            }
            else
            {
                lastSafeValues[theSender.Name] = theSender.Text;
            }
        }
        #endregion

        #region notes
        private void saveNotes(object sender, EventArgs e)
        {
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName.Length == 0)
                return;

            StreamWriter sw = new StreamWriter(saveFileDialog1.OpenFile());
            sw.WriteLine(notes.Text);
            sw.Flush();
            sw.Close();
        }

        private void loadNotes(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName.Length == 0)
                return;

            StreamReader sr = new StreamReader(openFileDialog1.OpenFile());
            notes.Text = sr.ReadToEnd();
            sr.Close();
        }
        private void evalNotes(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Regex.IsMatch(notes.Text, "eval\\(.+\\)"))
            {
                notes.Text = Regex.Replace(notes.Text, "eval\\(.+\\)", replaceEvals);
                notes.Text = notes.Text.Substring(0, notes.Text.Length - 1); // substring to remove the newline
                notes.SelectionStart = notes.Text.Length;
            }
        }

        private string replaceEvals(Match m)
        {
            string thestring = m.ToString().ToLower();
            try
            {
                thestring = thestring.Substring(5, thestring.Length - 6);
                thestring = numberAbbreviationHandler.replaceAbbreviations(thestring);
                DataTable dt = new DataTable();
                return numberAbbreviationHandler.getNumberEstimate(Decimal.Parse(dt.Compute(thestring, "").ToString()), 10);
            }
            catch (Exception) // not even bothering to try to regex the function
            {

            }
            return thestring;
        }

        #endregion

        #region timer
        
        private void startTimer(object sender, EventArgs e)
        {
            time timeLeft = time.timeFromString(textBox1.Text);
            timeLeft.message = textBox2.Text;
            if (!time.addTime(timeLeft))
                return;
            time final = time.addToCurrentTime(timeLeft);
            listBox1.Items.Add(String.Format("{0:00}:{1:00}:{2:00}-{3}", final.hours, final.minutes, final.seconds, final.message));
        }

        private void removeTimer(object sender, EventArgs e)
        {
            time.removeTime(listBox1.SelectedItem.ToString().Split('-')[1]);
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
        }

        private void onTimeEnd(string s)
        {
            if(InvokeRequired)
            {
                Invoke(new Action<string>(onTimeEnd), s);
                return;
            }

            tabControl1.SelectTab(2);
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            for(int i=0; i<listBox1.Items.Count; i++)
            {
                if (listBox1.Items[i].ToString().Split('-')[1].Equals(s))
                {
                    listBox1.Items[i] = listBox1.Items[i].ToString() + "-FINISHED";
                    break;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button11.Enabled = (listBox1.SelectedIndices.Count != 0);
        }

        string lastSafe = "";
        private void checkTime(object sender, EventArgs e)
        {
            if(!Regex.IsMatch(textBox1.Text, "^(\\d+:{1})*\\d*$"))
            {
                textBox1.Text = lastSafe;
                textBox1.SelectionStart = textBox1.Text.Length;
            } else
            {
                lastSafe = textBox1.Text;
            }
        }
        #endregion

        #region pinging
        private void enableButtons()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(enableButtons));
                return;
            }
            button12.Enabled = true;
            button13.Enabled = true;
        }

        private void scanConnections(object sender, EventArgs e)
        {
            button12.Enabled = false;
            button13.Enabled = false;
            richTextBox1.Clear();
            gamePinger gp = gamePinger.findOnPort(43594);
            if (gp != null)
            {
                gp.onPingResult += outputPingInformation;
                gp.finishedPinging += enableButtons;
                richTextBox1.AppendText(String.Format("Pinging {0} times with a {1} ms delay...\n", (int)numericUpDown1.Value, (int)numericUpDown2.Value));
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                gp.asyncPing((int)numericUpDown1.Value, (int)numericUpDown2.Value);
            }
            else
            {
                richTextBox1.AppendText("Couldn't find server\n");
            }
        }

        private void outputPingInformation(string ip, int[] latencies)
        {
            if(InvokeRequired)
            {
                Invoke(new Action<string, int[]>(outputPingInformation),ip, latencies);
                return;
            }
            lock(textBoxLock)
            {
                richTextBox1.AppendText("Server responses from " + ip + "\n");
                for (int i = 0; i < latencies.Length; i++)
                {
                    richTextBox1.AppendText("Ping response " + latencies[i] + " ms\n");
                }
                richTextBox1.AppendText("Average = " + gamePinger.averageLatency(latencies) + " ms\n");
                richTextBox1.AppendText("Std Deviation = " + gamePinger.standardDeviation(latencies) + " ms\n");
            }
        }
        
        Object syncLock = new object();
        Object textBoxLock = new object();
        SortedList<int[],string> results;

        private void findBestWorld(object sender, EventArgs e)
        {
            button12.Enabled = false;
            button13.Enabled = false;
            richTextBox1.Clear();
            results = new SortedList<int[], string>(new PingComparer());
            gamePinger gp = new gamePinger();
            gp.onPingResult += outputPingInformation;
            gp.onPingResult += addResult;
            gp.finishedPinging += printOrderedList;
            gp.finishedPinging += enableButtons;
            gp.pingGroup(jagexWorlds.americanWorldsToIp(), (int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value);
        }

        class PingComparer : IComparer<int[]>
        {
            public int Compare(int[] x, int[] y)
            {
                int result = gamePinger.averageLatency(x).CompareTo(gamePinger.averageLatency(y));
                return (result == 0) ? 1 : result;
            }
        }

        private void addResult(string ip, int[] latency)
        {
            lock(syncLock)
            {
                results.Add(latency, ip);
            }
        }

        private void printOrderedList()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(printOrderedList));
                return;
            }
            lock (textBoxLock)
            {
                foreach (var pair in results)
                {
                    richTextBox1.AppendText(pair.Value + " with average ping " + gamePinger.averageLatency(pair.Key) + " ms and std dev of " + gamePinger.standardDeviation(pair.Key) + " ms\n");
                }
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
            }
        }
        #endregion

    }
}
