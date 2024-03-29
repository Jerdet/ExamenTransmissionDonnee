﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Threading;
using System.Net;

using System.Net.NetworkInformation;

namespace ProjetFinalDessandeDetournayPecriauxWery
{
    public partial class ProjetFinal : Form
    {
        int compteurChoix = 0; //switch entre la température et les lux
        int compteurUsart = 0;
        private TcpListener tcpListener; //Objet pour gérer la connexion tcp
        private Thread listenThread; //Objet pour gérer le thread du tcp
        private delegate void ChangeLabelTcp(string str); //Creation du delegate pour le thread tcp
        private delegate void ChangeEtatGroup(bool valeur); //Creation du delegate pour afficher ou non le group des Pings
        private delegate void ReceiveSerial(string str); //Creation du delegate pour le thread du serialport
        string ipclient = "";
        string[] listePorts; //Tableau qui récupère la liste des ports
        /*float temperatureMax = float.MinValue;
        float temperatureMin = float.MaxValue;
        float temperatureTotal = 0;
        int temperatureNbTemp = 0;
        float temperatureMoyenne;
        */
        public ProjetFinal()
        {
            InitializeComponent();
            initialiseComboBoxSerial();
            Serveur();
        }
        //TCPServerEthernet.Server serveurTCP
        public void initialiseComboBoxSerial(){
            listePorts = System.IO.Ports.SerialPort.GetPortNames(); //Récupérer tout les ports série disponibles sur le PC
            choixPortSerial.DataSource = listePorts; //Les mettres dans la combobox
        }
        public void Serveur(){
            this.tcpListener = new TcpListener(IPAddress.Any, 45684);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }

        private void ChangeLabelText(string str) {
            //temperatureTotal += Single.Parse(str);
            //temperatureNbTemp++;
            //temperatureMoyenne = temperatureTotal / temperatureNbTemp;
            //if (temperatureMin > Single.Parse(str)) temperatureMin = Single.Parse(str);
            //if (temperatureMax < Single.Parse(str)) temperatureMax = Single.Parse(str);
            //temperatureMoyenne = temperatureTotal / temperatureNbTemp;
            donneeTemperature.Text = str;
            //donneeTempMax.Text = temperatureMax.ToString("R");
            //donneeTempMin.Text = temperatureMin.ToString("R");
            //donneeTempMoy.Text = temperatureMoyenne.ToString("R");
        }
        private void ChangeLabelLumiere(string str) {donneeLumiere.Text = str;}
        private void ChangeLabelPortSerie(string str) { portSerie.Text = str; }
        private void ChangeLabelEtatConnexion(string str) { messageEtatClient.Text = str; }
        private void ChangeLabelIPClient(string str) { donneeIpClient.Text = str; }
        private void ChangeEtatDuGroup(bool valeur) { pingGroup.Visible = valeur; }
        private void ChangeEtatGroupAll(bool valeur) { ethernet.Visible = valeur; usart.Visible = valeur; }

        private void ListenForClients()
        {
            this.tcpListener.Start(); //Initialisation du socket TCP comme sur le PIC

            while (true)
            {
                //Tant qu'un client ne s'est pas connecté
                TcpClient carte = this.tcpListener.AcceptTcpClient(); //Accepte une demande de connexion en attente
                //Création d'un thread pour garder la communication
                Thread threadCarte = new Thread(new ParameterizedThreadStart(HandleClientComm));
                threadCarte.Start(carte);
            }
        }


        private void HandleClientComm(object carte)
        {
            TcpClient tcpClient = (TcpClient)carte; //On caste le thread en type TcpClient
            NetworkStream clientStream = tcpClient.GetStream(); //Flux du client utilisé pour l'envoie et la réception de données

            byte[] message = new byte[4096];
            int byteslus;

            while (true)
            {
                byteslus = 0;

                //Connexion d'un client et réception en cours
                try
                {
                    this.Invoke(new ChangeLabelTcp(ChangeLabelEtatConnexion), "Client connecté"); //On signale la connexion d'un client en changeant le label avec un delegate
                    ipclient = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString(); //On récupère l'IP du client
                    this.Invoke(new ChangeLabelTcp(ChangeLabelIPClient), ipclient); //On change le label pour afficher l'IP
                    this.Invoke(new ChangeEtatGroup(ChangeEtatDuGroup), true); //Changement de l'état du groupe ping pour afficher le group ping lorsqu'il y a une connexion
                    byteslus = clientStream.Read(message, 0, 4096); //Tant qu'un client n'envoie pas de message
                }
                catch
                {
                    MessageBox.Show("Vérifier la connexion", "Erreur Socket", MessageBoxButtons.OK,MessageBoxIcon.Error);
                    //Erreur socket
                    break;
                }

                if (byteslus == 0)
                {
                    //Client déconnecté du serveur
                    break;
                }

                //Message recu avec succès
                ASCIIEncoding encoder = new ASCIIEncoding();
                string chaine;
                System.Diagnostics.Debug.WriteLine(encoder.GetString(message, 0, byteslus));
                chaine = encoder.GetString(message,0,byteslus); //Un genre de ftoa pour les bytes vers string
                compteurChoix++;
                if(compteurChoix%2 != 0)
                    this.Invoke(new ChangeLabelTcp(ChangeLabelText), chaine);
                else
                    this.Invoke(new ChangeLabelTcp(ChangeLabelLumiere), chaine);
            }

            tcpClient.Close();
        }
        
        private void dataReceiveSerial(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string stringRead = serialPort.ReadExisting(); //Lecture de ce qui est reçu sur le port série
            compteurUsart++;
            /*
            if (compteurUsart == 1)
                this.Invoke(new ReceiveSerial(ChangeLabelPortSerie), stringRead);
            else if (compteurUsart == 2)
                this.Invoke(new ReceiveSerial(ChangeLabelNumTel), stringRead);
            else
            {
                this.Invoke(new ReceiveSerial(ChangeLabelStatut), stringRead);
                compteurUsart = 0;
            }
            */
            this.Invoke(new ReceiveSerial(ChangeLabelPortSerie), stringRead);
            if (stringRead == "TATA" || stringRead == "TITI" || stringRead == "TOTO")
                this.Invoke(new ChangeEtatGroup(ChangeEtatGroupAll), true); //Si chaine ok -> affiche tout les groupeBox
            else
            {
                //Sinon affiche un message box d'erreur et ferme tout les groupbox
                this.Invoke(new ChangeEtatGroup(ChangeEtatGroupAll), false);
                MessageBox.Show("Mauvais utilisateur !");
            }
        }

        private void buttonPing_Click(object sender, EventArgs e)
        {
            string chaineReponsePing = "";
            Ping envoiPing = new Ping();
            PingOptions optionsPing = new PingOptions();

            optionsPing.DontFragment = true;

            //Créer un buffer de 32 octets de transmission de données
            string donneePing = "abcdefghijklmnopqrstuvwxyz012345";
            byte[] buffer = Encoding.ASCII.GetBytes(donneePing);
            int timeoutPing = 1000; //Important ! Le temps qu'on attent pour la réponse du ping
            PingReply receptionPing = envoiPing.Send(ipclient, timeoutPing,buffer,optionsPing); //Exécution du ping et réception sur l'objet de type PingReply
            if (receptionPing.Status == IPStatus.Success)
            {
                chaineReponsePing += "Status du Ping: " + receptionPing.Status + "\n";
                chaineReponsePing += "Adresse: " + receptionPing.Address.ToString() + "\n";
                chaineReponsePing += "TTL: " + receptionPing.Options.Ttl + "\n";
                chaineReponsePing += "Don't fragment: " + receptionPing.Options.DontFragment + "\n";
                chaineReponsePing += "Taille buffer: " + receptionPing.Buffer.Length;
                detailsPing.Text = chaineReponsePing;
            }
            else {
                chaineReponsePing += "Status du Ping: " + receptionPing.Status;
                detailsPing.Text = chaineReponsePing;
            }
        }

        private void messageEtat_Click(object sender, EventArgs e)
        {

        }

        private void messageEtatClient_Click(object sender, EventArgs e)
        {

        }

        private void ProjetFinal_Load(object sender, EventArgs e)
        {

        }

        private void buttonEnvoyer_Click(object sender, EventArgs e)
        {
            serialPort.Write(textID.Text + "\0"); //Envoi la donnée qui se trouve dans le textID.Text sur le serial port
        }

        private void buttonDeconnexion_Click(object sender, EventArgs e)
        {
            ethernet.Visible = false;
            usart.Visible = false;
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Environment.Exit(0);
        }

        private void buttonChoixPort_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen) serialPort.Close();
            if (choixPortSerial.SelectedIndex > -1)
            {
                serialPort.PortName = (string)choixPortSerial.SelectedItem;
                portSelectionne.Text = serialPort.PortName;
                portSelectionne.Visible = true;
                try
                {
                    serialPort.Open(); //Si une erreur se produit sur l'ouverture du port série, génère une exception et c'est traité en dessous
                    this.BackgroundImage = ProjetFinalDessandeDetournayPecriauxWery.Properties.Resources.rfidlogo;
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Opération invalide !");
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Accès non autorisé\nVérifier si le bootloader est ouvert !");
                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show("Port non reconnu\nEn choisir un autre");
                }
            }
            else {
                MessageBox.Show("Veuillez Selectionner un port COM");
            }
            listePorts = System.IO.Ports.SerialPort.GetPortNames();
            choixPortSerial.DataSource = listePorts;
        }
    }
}
