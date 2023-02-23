using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

namespace WPFConConnexionMySql
{
    public partial class MainWindow : Window
    {
        //Ahora vamos a invocar el modelo y lo asignamos a DataContext
        private ModelView modelo = new ModelView();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = modelo;
            //Ahora q ya tenemos registros en la BD's debemos hacer un metodo para cargar los datos de la Base
            modelo.LeerHistorico();
        }
        private void ComandoGuardar(object sender, RoutedEventArgs e)
        {//ahora creare el metodo para el guardado o actualizacion del modelo y de la BDs
            if (modelo.registros == null) modelo.registros = new ObservableCollection<Registro>();
            //Aqui solo valido q si el registro no existe, entonces procedemos a crearlo
            if (modelo.registros.Where(x => x.usuario == modelo.usuario).FirstOrDefault() == null)
            {
                modelo.registros.Add(new Registro
                {
                    usuario = modelo.usuario,
                    mail = modelo.mail,
                    edad = modelo.edad
                });
                //Ahora habiendo agregado el registro al modelo, debemos agregarlo a la Bds
                modelo.NuevoRegistro();
            }
            //ahora si el registro ya existe, solo debemos actualizar el registro
            else
            {
                foreach (Registro r in modelo.registros)
                {
                    if (r.usuario == modelo.usuario)
                    {
                        r.mail = modelo.mail;
                        r.edad = modelo.edad;
                        break;
                    }
                }
                //Ahora actualizamos el registro en la BDs
                modelo.ActualizarRegistro();
            }
        }
    }
    public class ModelView : INotifyPropertyChanged
    {
        #region VARIABLES
        public event PropertyChangedEventHandler? PropertyChanged;
        //Declarare la constante para la conexion a la BDs
        private const String cnstr = "server=localhost;uid=muser;pwd=admin1234;database=maptrack";
        //Ahora el modelo de la lista de registros a mostrar
        private ObservableCollection<Registro> _registros;
        private String _usuario = "";
        private String _mail = "";
        private String _edad = "";
        #endregion
        #region OBJETOS
        public ObservableCollection<Registro> registros
        {
            get
            {
                return _registros;
            }
            set
            {
                _registros = value;
                OnPropertyChange("registros");
            }
        }
        public String usuario
        {
            get
            {
                return _usuario;
            }
            set
            {
                _usuario = value;
                OnPropertyChange("usuario");
            }
        }
        public String mail
        {
            get
            {
                return _mail;
            }
            set
            {
                _mail = value;
                OnPropertyChange("mail");
            }
        }
        public String edad
        {
            get
            {
                return _edad;
            }
            set
            {
                _edad = value;
                OnPropertyChange("edad");
            }
        }
        #endregion
        //Ahora creare el metodo q se encargara de actualizar las propiedades en cada cambio
        private void OnPropertyChange(String propiedad)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propiedad));
        }
        public void NuevoRegistro()
        {
            String SQL = $"Insert into usuarios (usuario, mail, edad) values ('{usuario}', '{mail}', {edad});";
            //Ahora usaremos las clases de la libreria de MySQL para ejecutar los queries
            MySqlConnection con = new MySqlConnection(cnstr);
            con.Open();
            MySqlCommand cmd = new MySqlCommand(SQL, con);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
        }
        public void ActualizarRegistro()
        {
            String SQL = $"Update usuarios Set mail = '{mail}', edad = {edad} where usuario = '{usuario}';";
            MySqlConnection con = new MySqlConnection(cnstr);
            con.Open();
            MySqlCommand cmd = new MySqlCommand(SQL, con);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
        }
        public void LeerHistorico()
        {
            String SQL = $"select usuario, mail, edad from usuarios;";
            MySqlConnection con = new MySqlConnection(cnstr);
            con.Open();
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(SQL, con);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                if (registros == null) registros = new ObservableCollection<Registro>();
                foreach (DataRow i in dt.Rows)
                {
                    registros.Add(new Registro {
                        usuario = i[0].ToString(),
                        mail = i[1].ToString(),
                        edad = i[2].ToString(),
                    });
                }
            }
            dt.Dispose();
            da.Dispose();
            con.Close();
        }
    }
    //Ahora creamos el modelo de datos de la lista, debemos hacer lo mismo q el videmodel, debemos agregar el InotifyPropertyChanged
    public class Registro : INotifyPropertyChanged
    {
        //Como los objetos y variables solo deben ser usuario, mail y edad
        #region VARIABLES
        public event PropertyChangedEventHandler? PropertyChanged;
        private String _usuario = "";
        private String _mail = "";
        private String _edad = "";
        #endregion
        #region OBJETOS
        public String usuario
        {
            get
            {
                return _usuario;
            }
            set
            {
                _usuario = value;
                OnPropertyChange("usuario");
            }
        }
        public String mail
        {
            get
            {
                return _mail;
            }
            set
            {
                _mail = value;
                OnPropertyChange("mail");
            }
        }
        public String edad
        {
            get
            {
                return _edad;
            }
            set
            {
                _edad = value;
                OnPropertyChange("edad");
            }
        }
        #endregion
        //Ahora creare el metodo q se encargara de actualizar las propiedades en cada cambio
        private void OnPropertyChange(String propiedad)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propiedad));
        }
    }
}
