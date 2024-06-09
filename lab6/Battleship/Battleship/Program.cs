namespace Battleship
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new BattleShipGame());
            
        }
    }
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 450,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label()
            {
                Left = 20,
                Top = 20,
                Text = text,
                AutoSize = true,
                 // Максимальная ширина метки
            };

            TextBox textBox = new TextBox()
            {
                Left = 20,
                Top = textLabel.Bottom + 10,
                Width = 300
            };

            Button confirmation = new Button()
            {
                Text = "Сохранить",
                Left = 20,
                Height = 30,
                Width = 100,
                Top = textBox.Bottom + 10,
                DialogResult = DialogResult.OK
            };

            confirmation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}