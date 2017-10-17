using CommonErrorsKata.Shared;
using System.IO;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonErrorsKata
{
    public partial class CommonErrorsForm : Form
    {
        private const int MinAnswers = 15;
        private readonly AnswerQueue<TrueFalseAnswer> answerQueue;
        private readonly string[] files;
        private readonly SynchronizationContext synchronizationContext;
        private int i = 100;
        private string currentBaseName = null;
        private readonly string[] possibleAnswers = null;
        private readonly string[] fileNames = null;

        public CommonErrorsForm()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
            files = System.IO.Directory.GetFiles(Environment.CurrentDirectory +  @"..\..\..\ErrorPics");
            possibleAnswers = new string[] { "Missing File", "null instance", "divide by zero" };
            lstAnswers.DataSource = possibleAnswers;
            fileNames = new string[] { "object_ref.png", "object_ref_not_set.png", "divide_by_zero.png" };

            answerQueue = new AnswerQueue<TrueFalseAnswer>(MinAnswers);
            Next();
            lstAnswers.Click += LstAnswers_Click;
            StartTimer();
        }
        private async void StartTimer()
        {
            await Task.Run(() =>
            {
                for (i = 100; i > 0; i--)
                {
                    UpdateProgress(i);
                    Thread.Sleep(50);
                }
                Message("Need to be quicker on your feet next time!  Try again...");
            });
        }

        private void LstAnswers_Click(object sender, EventArgs e)
        {
            i = 100;
            var tokens = currentBaseName.Split(' ');

            if (tokens.Length == 0)
            {
                return;
            }
     
            var imageName = tokens[0];
            var index = Array.IndexOf(fileNames, imageName);
         
            if (index < 0 || index >= possibleAnswers.Length)
            {
                return;
            }
      
            var correctAnswer = possibleAnswers[index];
          
         Console.WriteLine(lstAnswers.SelectedItem.ToString());
           
             if (lstAnswers.SelectedItem.ToString() == correctAnswer)
             {
                answerQueue.Enqueue(new TrueFalseAnswer(true));
             }
             else
             {
                answerQueue.Enqueue(new TrueFalseAnswer(false));
             }

            Next();
          
        }

        private void Next()
        {
            if (answerQueue.Count >= MinAnswers && answerQueue.Grade >= 98)
            {
                MessageBox.Show("Congratulations you've defeated me!");
                Application.Exit();
                return;
            }
            label1.Text = answerQueue.Grade.ToString() + "%";
            var file = files.GetRandom();
            currentBaseName= Path.GetFileName(file);
            pbImage.ImageLocation = file;
        }

        public void UpdateProgress(int value)
        {
            synchronizationContext.Post(new SendOrPostCallback(x => {
                progress.Value = value;
            }), value);
        }
        public void Message(string value)
        {
            synchronizationContext.Post(new SendOrPostCallback(x => {
                MessageBox.Show(value);
            }), value);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
