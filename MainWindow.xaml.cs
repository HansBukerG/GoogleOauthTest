using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace GoogleTest02
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserCredential _credential;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GoogleSignInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string clientId = "";
                string clientSecret = "";

                _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret
                    },
                    new[] { "openid", "email", "profile" }, // Incluye el scope "openid"
                    "user", CancellationToken.None);

                // Refresca el token si está a punto de expirar
                if (_credential.Token.IsExpired(SystemClock.Default))
                {
                    await _credential.RefreshTokenAsync(CancellationToken.None);
                }

                // Obtener el ID Token (JWT) actualizado
                var idToken = _credential.Token.IdToken;
                Clipboard.SetText(idToken);

                MessageBox.Show("Inicio de sesión exitoso. ID Token copiado al portapapeles.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en el inicio de sesión: " + ex.Message);
            }
        }


        private async void GoogleSignOutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_credential != null && !string.IsNullOrEmpty(_credential.Token.RefreshToken))
                {
                    await _credential.RevokeTokenAsync(CancellationToken.None);
                    MessageBox.Show("Sesión cerrada y token revocado.");
                }
                else
                {
                    MessageBox.Show("No hay una sesión activa para cerrar.");
                }
            }
            catch (TokenResponseException ex) when (ex.Error.Error == "invalid_token")
            {
                MessageBox.Show("El token ya ha expirado o ha sido revocado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error: " + ex.Message);
            }
            finally
            {
                _credential = null;
            }
        }
    }
}
