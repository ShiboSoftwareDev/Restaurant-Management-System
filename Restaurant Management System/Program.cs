namespace Restaurant_Management_System
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            bool loginSuccess = false;
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    loginSuccess = true;
                }
            }
            if (loginSuccess)
            {
                Application.Run(new Form1());
            }
        }
    }
}