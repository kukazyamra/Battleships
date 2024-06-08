namespace Battleship
{
    public partial class Form1 : Form
    {
        private bool[] placements;
        private const int gridSize = 10;
        private const int cellSize = 35;
        private const int offset = 40;
        private const int margin = 1;
        private bool finished;
        private Button changePlayer;
        private Game game;

        public Form1()
        {
            Button finishGame = new Button();
            finishGame.AutoSize = true;
            finishGame.Text = "Завершить игру";
            finishGame.Click += FinishGame_Click;
            finishGame.Location = new Point(40, 500);
            finishGame.Tag = "endGame";
            this.Controls.Add(finishGame);
            placements = new bool[2];
            finished = false;
            game = new Game();
            string name1 = Prompt.ShowDialog("Введите имя первого игрока", "Вход в игру");

            while (!game.ValidateName(name1))
            {
                name1 = Prompt.ShowDialog("Некорректный ввод имени. Попробуйте еще раз", "Вход в игру");
            }

            var player1 = new Player(name1);
            game.Players[0] = player1;
            string name2 = Prompt.ShowDialog("Введите имя второго игрока", "Вход в игру");

            while (!game.ValidateName(name2))
            {
                name2 = Prompt.ShowDialog("Некорректный ввод имени. Попробуйте еще раз", "Вход в игру");
            }
            var player2 = new Player(name2);
            game.Players[1] = player2;
            InitializeComponent();
            GetPlacement(player1);
            AddLabel($"Расстановка игрока {player1.Name}", 25, 20);
        }

        private void DrawGrid(Player player, int offset, string mode)
        {
            var label = new Label();
            if (mode == "placement")
            {
                label.Text = $"Расстановка игрока {player.Name}";
                label.Location = new Point(25, 20);
                label.AutoSize = true;
                label.Font = new Font(label.Font.FontFamily, 20);
                this.Controls.Add(label);
            }
            var buttons = new List<Button>();

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Button button = new Button();
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 1;
                    button.Size = new Size(cellSize, cellSize);
                    button.Location = new Point(offset + j * (cellSize + margin), 80 + i * (cellSize + margin));
                    button.Tag = $"{i},{j}";
                    if (mode == "placement")
                    {
                        button.Click += (sender, e) => Placement_Click(sender, e, player);
                    }
                    else if (mode == "enemy")
                    {
                        button.Tag = $"{i},{j},enemy";

                        button.Click += (sender, e) => Enemy_Click(sender, e, player);
                    }
                    switch (player.Board.Cells[i, j].CellState)
                    {
                        case CellStates.Ship:
                            if (mode == "enemy")
                            {
                                button.BackColor = Color.White;
                            }
                            else
                            {
                                button.BackColor = Color.SlateBlue;
                            }
                            break;
                        case CellStates.DamagedShip:
                            button.BackColor = Color.Orange;
                            break;
                        case CellStates.DestroyedShip:
                            button.BackColor = Color.Red;
                            break;
                        case CellStates.Miss:
                            button.BackColor = Color.Aqua;
                            break;
                        default:
                            button.BackColor = Color.White;
                            break;
                    }
                    buttons.Add(button);
                    this.Controls.Add(button);
                }
            }
        }

        private void AddLabel(string text, int x, int y)
        {
            var label = new Label();
            label.Text = text;
            label.Location = new Point(x, y);
            label.AutoSize = true;
            label.Font = new Font(label.Font.FontFamily, 20);
            this.Controls.Add(label);
        }

        private void GetPlacement(Player player)
        {
            DrawGrid(player, offset, "placement");
            var savePlacementButton = new Button();
            savePlacementButton.Text = "Сохранить расстановку";
            savePlacementButton.Click += (sender, e) => SavePlacementButton_Click(sender, e, player);
            savePlacementButton.Size = new Size(150, 50);
            savePlacementButton.Location = new Point(500, 60);
            this.Controls.Add(savePlacementButton);
        }
        private void RedrawEnemyBoard(Player player)
        {
            var buttons = this.Controls.OfType<Button>()
                        .Where(button => button.Tag != null && button.Tag.ToString().Contains("enemy"));
            foreach (var button in buttons)
            {
                var tag = button.Tag.ToString().Split(',');
                var i = int.Parse(tag[0]); var j = int.Parse(tag[1]);
                switch (player.Board.Cells[i, j].CellState)
                {
                    case CellStates.Ship:
                        button.BackColor = Color.White;
                        break;
                    case CellStates.DamagedShip:
                        button.BackColor = Color.Orange;
                        break;
                    case CellStates.DestroyedShip:
                        button.BackColor = Color.Red;
                        break;
                    case CellStates.Miss:
                        button.BackColor = Color.Aqua;
                        break;
                    default:
                        button.BackColor = Color.White;
                        break;
                }
            }
        }



        public void Turn(Player player1, Player player2)
        {
            changePlayer = new Button();
            changePlayer.Enabled = false;
            changePlayer.Text = "Передать ход другому игроку";
            changePlayer.AutoSize = true;
            changePlayer.Location = new Point(500, 35);
            changePlayer.Click += (sender, e) => ChangePlayer_Click(sender, e, player1, player2);

            this.Controls.Add(changePlayer);
            AddLabel($"Ход игрока {player1.Name}", 25, 20);
            DrawGrid(player1, 40, "own");
            DrawGrid(player2, 500, "enemy");
        }

        private void ChangePlayer_Click(object sender, EventArgs e, Player player1, Player player2)
        {
            finished = false;
            ClearControls();
            Turn(player2, player1);
        }

        public void Placement_Click(object sender, EventArgs e, Player player)
        {

            Button button = sender as Button;
            var tag = button.Tag.ToString().Split(',');
            var cell = player.Board.Cells[int.Parse(tag[0]), int.Parse(tag[1])];
            if (cell.CellState == CellStates.Empty)
            {
                cell.CellState = CellStates.Ship;
                button.BackColor = Color.SlateBlue;
            }
            else
            {
                cell.CellState = CellStates.Empty;
                button.BackColor = Color.White;
            }
        }

        public void Enemy_Click(object sender, EventArgs e, Player player)
        {
            if (finished) return;
            Button button = sender as Button;
            var tag = button.Tag.ToString().Split(',');
            var cell = player.Board.Cells[int.Parse(tag[0]), int.Parse(tag[1])];
            var coordinates = (int.Parse(tag[0]), int.Parse(tag[1]));
            if (cell.CellState == CellStates.Empty)
            {
                finished = true;
                changePlayer.Enabled = true;
            }
            game.HandleShot(coordinates, player);

            RedrawEnemyBoard(player);
            var winner = game.CheckWin();
            if (winner != null)
            {
                MessageBox.Show("Победитель: " + winner.Name, "Победа!");
                finished = true;
                ClearControls();
                AddLabel($"Поле игрока {game.Players[0].Name}", 25, 20);
                AddLabel($"Поле игрока {game.Players[1].Name}", 500, 20);
                AddLabel($"Победитель: {winner.Name}", 350, 500);
                DrawGrid(game.Players[0], 40, "own");
                DrawGrid(game.Players[1], 500, "own");
            }

        }
        private void SavePlacementButton_Click(object? sender, EventArgs e, Player player)
        {
            if (player.Board.ValidatePlacements())
            {
                if (!placements[0])
                {
                    MessageBox.Show("Расстановка успешно сохранена.", "Успешно!");
                    placements[0] = true;
                    ClearControls();
                    GetPlacement(game.Players[1]);
                    AddLabel($"Расстановка игрока {game.Players[1].Name}", 25, 20);
                }
                else
                {
                    MessageBox.Show("Расстановка успешно сохранена.", "Успешно!");
                    ClearControls();
                    Turn(game.Players[0], game.Players[1]);
                }
            }
            else
            {
                MessageBox.Show("Расстановка некорректна");
            }
        }

        private void FinishGame_Click(object? sender, EventArgs e)
        {

            this.Close();
        }

        private void ClearControls()
        {
            var controls = Controls.OfType<Control>()
                           .Where(c => (c is Button && (c.Tag == null || c.Tag.ToString() != "endGame")) || c is Label)
                           .ToArray();

            foreach (var control in controls)
            {
                Controls.Remove(control);
            }
        }


    }

}
