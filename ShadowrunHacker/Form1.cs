using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShadowrunHacker {
	public partial class Panel : Form {
		int dc = 0;//Difficulty Class
		int position = 0;
		List<int> pattern = new List<int>();
		Dictionary<int,Button> buttons = new Dictionary<int,Button>();
		List<Button> LEDs = new List<Button>();
		Random rand = new Random();
		bool playback = false;
		double time;
		public Panel() {
			InitializeComponent();
		}

		private void GUIPlayback() {
			playback = true;
			foreach (int i in pattern) {
				Thread.Sleep(200);
				blinkenight(buttons[i],Color.DarkBlue);
			}
			playback = false;
		}

		private void GUIPlayback(int i, Color c) {
			playback = true;
			blinkenight(buttons[i], c);
			playback = false;
		}

		private void testCorrect(int buttonPressed) {
			if (playback) return;
			if (pattern!=null && pattern[position] == buttonPressed) position++;
			else {
				//IF THE WRONG BUTTON IS PRESSED
				//Notify user of wrong button pressed via button flash
				//blinkenight(buttons[buttonPressed], Color.Red);//NOT CURRENTLY WORKING
				//Wait some time as punishment.
				//Thread.Sleep(200);//NOT CURRENTLY WORKING?
				reset();//Reset may not ben working
				//BUZZER NOIZE OR SOMETHING?
				new Thread(GUIPlayback).Start();
			}
			if (position >= pattern.Count) {
				LEDs[dc - 1].BackColor = Color.Green;
				dc++;
				reset();
				if (dc == 8) {
					timer1.Stop();
					buttonStart.Enabled = true;
					reset();
					time = 45.0;
					MessageBox.Show("CONGRATULATIONS", "YOU SOLVED ALL THE PUZZLES!");
				}
				else new Thread(GUIPlayback).Start();
			}
		}

		private void blinkenight(Button b, Color c) {
			b.BackColor = c;
			Thread.Sleep(200);
			b.BackColor = Color.Teal;
		}

		private void Panel_Load(object sender, EventArgs e) {
			buttons.Add(1, button1); buttons.Add(2, button2); buttons.Add(3, button3);
			buttons.Add(4, button4); buttons.Add(5, button5); buttons.Add(6, button6);
			buttons.Add(7, button7); buttons.Add(8, button8); buttons.Add(9, button9);
			LEDs.Add(LED1); LEDs.Add(LED2); LEDs.Add(LED3); LEDs.Add(LED4); LEDs.Add(LED5);
			LEDs.Add(LED6); LEDs.Add(LED7);
			time = 45.0;
		}

		private void level() {
			int temp = 0;
			if (dc < 1 || dc > 8) dc = 1;
			if (dc <= 2 && dc >= 1)	temp = 4;
			else if (dc <= 4 && dc >= 3) temp = 5;
			else if (dc <= 6 && dc >= 5) temp = 6;
			else temp = 7;
			for (int i = 0; i < temp; i++) pattern.Add(rand.Next(1, 10));
		}

		private void buttonStart_Click(object sender, EventArgs e) {
			resetLEDs();
			buttonStart.Enabled = false;
			dc++;
			pattern = new List<int>();
			level();
			timer1.Start();
			new Thread(GUIPlayback).Start();
			
		}

		private void reset() {
			//Do I want to change the dc on a reset?
			//int dc = 0;
			position = 0;
			pattern = null;
			pattern = new List<int>();
			level();
			//Random rand = new Random();
			//bool playback = false;
		}

		private void ButtonClickedHandler(object sender, EventArgs e){
			Button b = sender as Button;
			if(pattern.Count>0)testCorrect(Int32.Parse(b.Text));
		}

		private void LEDHandler() {
			if (dc > 6 || dc < 1) return;
			LEDs[dc - 1].BackColor = Color.Green;
		}

		private void resetLEDs() {
			foreach (Button LED in LEDs) LED.BackColor = Color.Red;
		}

		private void timer1_Tick(object sender, EventArgs e){
			if (time < .05) {
				timer1.Stop();
				buttonStart.Enabled = true;
				reset();
				time = 45.0;
				MessageBox.Show("TOO BAD","YOU RAN OUT OF TIME!");
			}
			time -= .1;
			string output = String.Format("Time Remaining: {0:0.00}", time);
			timerLabel.Text = output;
		}
	}
}
