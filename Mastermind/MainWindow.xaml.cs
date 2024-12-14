﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Mastermind
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer titleUpdate = new DispatcherTimer();
        List<StackPanel> stackPanels = new List<StackPanel>();
        int timesvalidated;
        public MainWindow()
        {
            InitializeComponent();
            StartGame();
            Data.GenerateRandomColorCode();
            FillComboBoxes();

            titleUpdate.Tick += new EventHandler(Update);
            titleUpdate.Interval = new TimeSpan(0, 0, 1);
            titleUpdate.Start();

            stackPanels.Add(StackColor1);
            stackPanels.Add(StackColor2);
            stackPanels.Add(StackColor3);
            stackPanels.Add(StackColor4);
            timesvalidated = 0;
        }
        public void Update(Object sender, EventArgs e)
        {
            this.Title = $"Poging {Data.Attempts}";
            LblAttempts.Content = $"Attempt: {Data.Attempts}";
            LblPlayerName.Content = $"Player: {Data.PlayerNames[Data.CurrentPlayer]}";
            LblScore.Content = $"Score: {Data.Score}";
            if (Data.Attempts - timesvalidated > 1)
            {
                foreach(StackPanel stack in stackPanels)
                {
                    if(stack.Children.Count - timesvalidated > 0)
                    {
                        stack.Children.RemoveAt(timesvalidated);
                    }

                    Label label = new Label();
                    label.Height = 32;
                    label.Width = 32;
                    label.Margin = new Thickness(5, 0, 5, 0);
                    label.Background = Brushes.DarkSlateGray;

                    stack.Children.Add(label);
                }
                ClearComboBoxSelection();
                Data.ResetBooleans();
                timesvalidated++;
            }
        }
        private void FillComboBoxes()
        {
            foreach(var color in Data.Colors)
            {
                CboColor1.Items.Add(color.Value);
                CboColor2.Items.Add(color.Value);
                CboColor3.Items.Add(color.Value);
                CboColor4.Items.Add(color.Value);
            }
        }
        private void CboColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbo = sender as ComboBox;
            if(cbo.SelectedIndex != -1)
            {
                if (cbo.Name.Equals("CboColor1"))
                {
                    AddOrChangeLabel(StackColor1, cbo, Data.Color1LabelAdded);
                    Data.Color1LabelAdded = true;
                }
                else if (cbo.Name.Equals("CboColor2"))
                {
                    AddOrChangeLabel(StackColor2, cbo, Data.Color2LabelAdded);
                    Data.Color2LabelAdded = true;
                }
                else if (cbo.Name.Equals("CboColor3"))
                {
                    AddOrChangeLabel(StackColor3, cbo, Data.Color3LabelAdded);
                    Data.Color3LabelAdded = true;
                }
                else if (cbo.Name.Equals("CboColor4"))
                {
                    AddOrChangeLabel(StackColor4, cbo, Data.Color4LabelAdded);
                    Data.Color4LabelAdded = true;
                }
            }
        }
        private void AddOrChangeLabel(StackPanel stackPanel,ComboBox comboBox, bool labelAdded)
        {
            if (!labelAdded)
            {
                Label label = new Label();
                label.Height = 32;
                label.Width = 32;
                label.Margin = new Thickness(5, 0, 5, 0);

                label.Background = GetColorBrush(comboBox);

                stackPanel.Children.Add(label);
            }
            else
            {
                Label label = (Label)stackPanel.Children[stackPanel.Children.Count - 1];
                label.Background = GetColorBrush(comboBox);
            }

        }
        private SolidColorBrush GetColorBrush(ComboBox comboBox)
        {
            SolidColorBrush brush = Data.Colors.Where(x => x.Value.Equals(comboBox.SelectedValue))
                                   .Select(x => x.Key).First();
            return brush;
        }
        private void BtnValidateCode_Click(object sender, RoutedEventArgs e)
        {
            if(CboColor1.SelectedIndex != -1 && CboColor2.SelectedIndex != -1 && CboColor3.SelectedIndex != -1 && CboColor4.SelectedIndex != -1)
            {
                timesvalidated++;
                Data.StopCountdown();
                List<int> points = Data.ValidateColorCode(new List<string> { CboColor1.SelectedValue.ToString(), CboColor2.SelectedValue.ToString(), CboColor3.SelectedValue.ToString(), CboColor4.SelectedValue.ToString() });
                List<StackPanel> stackPanels = new List<StackPanel>() { StackColor1, StackColor2, StackColor3, StackColor4 };
                for (int i = 0; i < stackPanels.Count; i++)
                {
                    ChangeBorder(stackPanels[i], points[i]);
                }
                if (!Data.CheckGameOver())
                {
                    int score = 0;
                    foreach (int point in points)
                    {
                        score += point;
                    }
                    if (score == 8)
                    {
                        if (Data.CurrentPlayer < Data.PlayerNames.Count - 1)
                        {
                            MessageBox.Show($"You Managed to crack the right code,your score is {Data.Score} points! \nNow It's {Data.PlayerNames[Data.CurrentPlayer + 1]} his/her turn", Data.PlayerNames[Data.CurrentPlayer]);
                            Data.Highscores.SetValue($"{Data.PlayerNames[Data.CurrentPlayer]} - {Data.Attempts} pogingen - {Data.Score}/100", Data.HighscoresSaved);
                            Data.HighscoresSaved++;
                            Data.CurrentPlayer++;
                            ClearUI();
                            Data.ResetGame();
                            Data.StopCountdown();
                            Data.StartCountdown();
                        }
                        else
                        {
                            MessageBox.Show($"You Managed to crack the right code,your score is {Data.Score} points!", Data.PlayerNames[Data.CurrentPlayer]);
                            Data.Highscores.SetValue($"{Data.PlayerNames[Data.CurrentPlayer]} - {Data.Attempts} pogingen - {Data.Score}/100", Data.HighscoresSaved);
                            Data.HighscoresSaved++;
                        }
                    }
                    else
                    {
                        Data.StartCountdown();
                    }
                }
                else
                {
                    if (Data.CurrentPlayer < Data.PlayerNames.Count - 1)
                    {
                        MessageBox.Show($"You did not manage to crack the right code, the correct code was {string.Join(" ", Data.ColorCode)}. Better luck next time! \nNow It's {Data.PlayerNames[Data.CurrentPlayer + 1]} his/her turn", Data.PlayerNames[Data.CurrentPlayer]);
                        Data.CurrentPlayer++;
                        ClearUI();
                        Data.ResetGame();
                        Data.StopCountdown();
                        Data.StartCountdown();
                    }
                    else
                    {
                        MessageBox.Show($"You did not manage to crack the right code, the correct code was {string.Join(" ", Data.ColorCode)}. Better luck next time!", Data.PlayerNames[Data.CurrentPlayer]);
                    }
                        
                }
                ClearComboBoxSelection();
                Data.IncreaseAttempst();

            }
            else
            {
                MessageBox.Show("Make sure to select a color at all positions.", "Error");
            }
        }
        private void ChangeBorder(StackPanel stackPanel, int points)
        {
            if (points == 2)
            {
                Label label = stackPanel.Children[stackPanel.Children.Count - 1] as Label;
                label.BorderBrush = Brushes.DarkRed;
                label.BorderThickness = new Thickness(3);
            }
            else if (points == 1)
            {
                Label label = stackPanel.Children[stackPanel.Children.Count - 1] as Label;
                label.BorderBrush = Brushes.Wheat;
                label.BorderThickness = new Thickness(3);
            }
        }
        private void ClearComboBoxSelection()
        {
            CboColor1.SelectedIndex = -1;
            CboColor2.SelectedIndex = -1;
            CboColor3.SelectedIndex = -1;
            CboColor4.SelectedIndex = -1;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12 && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                Data.ToggleDebug(ColorCodeTextbox);
            }
        }

        private void ClearUI()
        {
            foreach (StackPanel stack in stackPanels)
            {
                stack.Children.Clear();
            }
            ColorCodeTextbox.Clear();
            timesvalidated = 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to close the game?", "Are you sure?", MessageBoxButton.YesNo);
            if(result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void MnuAfsluiten_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MnuNieuwSpel_Click(object sender, RoutedEventArgs e)
        {
            ClearUI();
            Data.ResetGame();
            StartGame();
        }

        private void MnuHighscores_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string score in Data.Highscores)
            {
                if (!(score is null))
                {
                    sb.AppendLine(score);
                }
            }
            MessageBox.Show(sb.ToString(), "Mastemind highscores");
        }

        private void MnuAantalPogingen_Click(object sender, RoutedEventArgs e)
        {
            string input  = Interaction.InputBox("How many attempts would you like to have?", "Amount of attempts", "10", 500);
            int pogingen = 0;
            while (string.IsNullOrEmpty(input) || !int.TryParse(input, out pogingen) || pogingen > 20 || pogingen < 3)
            {
                MessageBox.Show("Please select a number between 3 and 20!", "Invalid number");
                input = Interaction.InputBox("How many attempts would you like to have?", "Amount of attempts", "10", 500);
            }
            Data.PossibleAttempts = pogingen;
            ClearUI();
            Data.ResetGame();
            Data.StartCountdown();
        }

        private void StartGame()
        {
            List<string> names = new List<string>();
            bool addAnotherPlayer = true;

            names.Add(AskName());
            while (addAnotherPlayer)
            {
                MessageBoxResult result = MessageBox.Show("Would you like to add another player?", "Select Players", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    names.Add(AskName());
                }
                else
                {
                    addAnotherPlayer = false;
                }
            }
            Data.PlayerNames = names;
            Data.AmountOfPlayers = names.Count;
            Data.CurrentPlayer = 0;
            Data.StartCountdown();
        }

        private string AskName()
        {
            string playerName = Interaction.InputBox("What is your name?", "Player Name", "", 500);
            while (string.IsNullOrEmpty(playerName))
            {
                MessageBox.Show("Please select a valid name!", "Invalid Name");
                playerName = Interaction.InputBox("What is your name?", "Player Name", "", 500);
            }
            return playerName;
        }
    }
}