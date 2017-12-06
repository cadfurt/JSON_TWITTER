using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;

namespace C_JSON_VISUAL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnCancelar.Enabled = false;
            btnCsv.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnCancelar.Enabled = true;
            btnFiltrar.Enabled = false;
            bgWorkerIndeterminada.RunWorkerAsync();
           
            //define a progressBar para Marquee
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 4;

            //informa que a tarefa esta sendo executada.
            label3.Text = "Processando...";

        }

        delegate void teste(string path);


        private void populaTxt(string path)
        {
            CheckForIllegalCrossThreadCalls = false;

            path = txtDiretorio.Text;

            dynamic textoToJson(string pArquivo)
            {
                using (StreamReader arquivo = File.OpenText(pArquivo))
                using (JsonTextReader leitor = new JsonTextReader(arquivo))
                {
                    dynamic resultado = JToken.ReadFrom(leitor);
                    return resultado;
                }
            }

            dynamic json_tweeter = textoToJson(@path);

            StringBuilder sb = new StringBuilder();
            
                richTextBox1.Clear();
                richTextBox1.Text = "user name" + " | " + "created_at" + " | " + "favourites_count" + " | " + "followers_count" + " | " + "friends_count" + " | " +
                                "statuses_count" + " | " + "text" + " || " + "\n";

                for (int i = 0; i < json_tweeter.Count; i++)
                {
                    richTextBox1.Text += (json_tweeter[i]["user"]["name"] + " | " +
                                            json_tweeter[i]["created_at"] + " | " +
                                            json_tweeter[i]["user"]["favourites_count"] + " | " +
                                            json_tweeter[i]["user"]["followers_count"] + " | " +
                                            json_tweeter[i]["user"]["friends_count"] + " | " +
                                            json_tweeter[i]["user"]["statuses_count"] + " | " +
                                            json_tweeter[i]["text"] + " || " +
                                            "\n");

                    Console.WriteLine("id: " + json_tweeter[i]["id"]);
                    Console.WriteLine(json_tweeter[i]["user"]["name"]);
                }
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string path2 = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory.ToString()) + "tweeter.CSV";
            System.IO.File.WriteAllText(path2, richTextBox1.Text);
            MessageBox.Show("Arquivo criado em : " + path2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.ShowDialog();
        }

        private void bgWorkerIndeterminada_DoWork(object sender, DoWorkEventArgs e)
        {

            //executa a tarefa a primeira vez
            populaTxt(txtDiretorio.Text);
            //Verifica se houve uma requisição para cancelar a operação.
            if (bgWorkerIndeterminada.CancellationPending)
            {
                //se sim, define a propriedade Cancel para true
                //para que o evento WorkerCompleted saiba que a tarefa foi cancelada.
                MessageBox.Show("CANCELAAA");
               e.Cancel = true;
                return;
            }

           
        }

        private void bgWorkerIndeterminada_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Caso cancelado...
            if (e.Cancelled)
            {
                // reconfigura a progressbar para o padrao.
                progressBar1.MarqueeAnimationSpeed = 0;
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.Value = 0;

                //caso a operação seja cancelada, informa ao usuario.
                label3.Text = "Operação Cancelada pelo Usuário!";

                //habilita o botao cancelar
                btnCancelar.Enabled = true;
                
            }
            else if (e.Error != null)
            {
                //informa ao usuario do acontecimento de algum erro.
                label3.Text = "Aconteceu um erro durante a execução do processo!";

                // reconfigura a progressbar para o padrao.
                progressBar1.MarqueeAnimationSpeed = 0;
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.Value = 0;
            }
            else
            {
                //informa que a tarefa foi concluida com sucesso.
                label3.Text = "Tarefa Concluida com sucesso!";

                //Carrega todo progressbar.
                progressBar1.MarqueeAnimationSpeed = 0;
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.Value = 100;
                btnCsv.Enabled = true;
                btnCancelar.Enabled = false;
                btnFiltrar.Enabled = false;
            }
            
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (bgWorkerIndeterminada.IsBusy)
            {
                // notifica a thread que o cancelamento foi solicitado.
                // Cancela a tarefa DoWork 
                bgWorkerIndeterminada.CancelAsync();
                               
            }

            //desabilita o botão cancelar.
            btnCancelar.Enabled = false;
            label3.Text = "Cancelando...";
        }
    }
}
