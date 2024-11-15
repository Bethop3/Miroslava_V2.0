using System;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace Miroslava_V2._0
{
    public partial class Form1 : MaterialSkin.Controls.MaterialForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Llamar a la función para detectar computadoras al cargar el formulario
            DetectarComputadorasEnRed();
        }

        private void DetectarComputadorasEnRed()
        {
            // Crear DataTable para mostrar en el DataGridView
            DataTable table = new DataTable();
            table.Columns.Add("Nombre del Dispositivo");
            table.Columns.Add("Dirección IP");

            // Obtener la IP base de la red local
            string ipBase = ObtenerIPBase();

            // Escanear la red local (del .1 al .254)
            for (int i = 1; i <= 254; i++)
            {
                string ip = $"{ipBase}.{i}";

                // Crear un objeto Ping para cada dirección IP
                Ping ping = new Ping();
                ping.PingCompleted += (sender, e) =>
                {
                    if (e.Reply != null && e.Reply.Status == IPStatus.Success)
                    {
                        try
                        {
                            // Obtener el nombre del host a partir de la IP
                            string hostName = Dns.GetHostEntry(e.Reply.Address).HostName;
                            // Agregar los datos al DataTable
                            table.Rows.Add(hostName, e.Reply.Address.ToString());
                            // Mostrar los datos en el DataGridView llamado "Computadoras"
                            Computadoras.Invoke((MethodInvoker)(() => Computadoras.DataSource = table));
                        }
                        catch
                        {
                            table.Rows.Add("Desconocido", e.Reply.Address.ToString());
                        }
                    }
                };

                // Enviar ping de forma asincrónica
                ping.SendAsync(ip, 1000, null);
            }
        }

        private string ObtenerIPBase()
        {
            string localIP = "";
            foreach (IPAddress ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }

            // Obtener la base de la IP (por ejemplo, "192.168.1")
            string[] partes = localIP.Split('.');
            return $"{partes[0]}.{partes[1]}.{partes[2]}";
        }


    }
}
