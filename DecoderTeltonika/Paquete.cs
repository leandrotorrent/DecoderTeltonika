using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoderTeltonika
{
    class Paquete
    {
        private string paquete;

        public Paquete(string paquete)
        {
            this.paquete = paquete;
        }

        /* Estructura paquete UDP Teltonika
         * Packet length = 2B
         * Packet Id = 2B
         * Packet Type = 1B
         * AVL packet id = 1B
         * IMEI Length= 2B
         * IMEI = 
         * 
         * Codec ID (AVL packet ID) = 1B 8E
         * Cantidad de elementos AVL Data = 1B
         * 
         * AVL Data:
         *  Timestamp = 8B
         *  Priority = 1B
         *  GPS = 15B
         *      Longitud = 4B
         *      Latitud = 4B
         *      Altura = 2B
         *      Ángulo = 2B
         *      Satelites = 1B
         *      Velocidad = 2B
         *  I/O = 
         *      Event IO ID = 2B
         *      Cantidad de elementos IO = 2B
         *      N1 Cantidad de elementos de 1B = 2B
         *      ID del 1er elemento IO de 1B = 2B
         *      Valor del 1er elemento IO de 1B = 1B
         *      ...
         *      ...
         *      ...
         *      N2 Cantidad de elementos de 2B = 2B
         *      ID del 1er elemento IO de 2B = 2B
         *      Valor del 1er elemento IO de 2B = 2B
         *      ...
         *      ...
         *      ...
         *      N4 Cantidad de elementos de 4B = 2B
         *      ID del 1er elemento IO de 4B = 2B
         *      Valor del 1er elemento IO de 2B = 4B
         *      ...
         *      ...
         *      ...
         *      N8 Cantidad de elementos de 8B = 2B
         *      ID del 1er elemento IO de 8B = 2B
         *      Valor del 1er elemento IO de 8B = 8B
         *      ...
         *      ...
         *      ...
         *      NX Cantidad de elementos de longitud variable = 2B
         *      ID del 1er elemento IO de XB = 2B
         *      Longitud del Elemento = 2B
         *      Valor = Definido por la longitud
         *      
         *      
         *      
         *      
         * 
         */

        public Dictionary<string, string> Parsear()
        {
            Dictionary<string, string> diccionarioTeltonika = new Dictionary<string, string>();
            Dictionary<string, string> diccionarioError = new Dictionary<string, string>();
            diccionarioError.Add("E", "E");

            string packetID = "CAFE"; //Para detectar un paquete valido.
            if (paquete.Contains(packetID))
            {
                


                string tramaTeltonika = paquete.Substring(paquete.IndexOf(packetID) - 4);
                //Console.WriteLine(tramaTeltonika);
                string c_packetLength = "Largo del paquete UDP";
                string packetLength = Convert.ToInt32(tramaTeltonika.Substring(0, 4), 16).ToString();
                string c_packetType = "Packet type";
                string packetType = Convert.ToInt32(tramaTeltonika.Substring(8, 2), 16).ToString();
                string c_avlPacketID = "AVL Packet ID";
                string avlPacketID = Convert.ToInt32(tramaTeltonika.Substring(10, 2), 16).ToString();
                string c_imeiLength = "Largo del IMEI";
                string imeiLength = Convert.ToInt32(tramaTeltonika.Substring(12, 4), 16).ToString();
                //string imeiLength = "15";
                string c_imei = "IMEI";
                //string imei = Convert.ToUInt64(tramaTeltonika.Substring(16, 30), 16).ToString();
                string preimei = tramaTeltonika.Substring(16, 30); // Hacer luego la conversion a decimal
                string imei = preimei.Substring(2, 2) + preimei.Substring(5, 1) + preimei.Substring(7, 1) + preimei.Substring(9, 1)
                                + preimei.Substring(11, 1) + preimei.Substring(13, 1) + preimei.Substring(15, 1) + preimei.Substring(17, 1)
                                + preimei.Substring(19, 1) + preimei.Substring(21, 1) + preimei.Substring(23, 1) + preimei.Substring(25, 1)
                                + preimei.Substring(27, 1) + preimei.Substring(29, 1);
                string c_codecID = "Codec ID";
                string codecID = Convert.ToInt32(tramaTeltonika.Substring(46, 2), 16).ToString();
                //Console.WriteLine(codecID);
                string c_cantidadElementosAvlData = "Cantidad de Elementos AVL Data";
                int cantidadElementosAvlData = Convert.ToInt32(tramaTeltonika.Substring(48, 2), 16);

                diccionarioTeltonika.Add(c_packetLength, packetLength);
                diccionarioTeltonika.Add(c_packetType, packetType);
                diccionarioTeltonika.Add(c_avlPacketID, avlPacketID);
                diccionarioTeltonika.Add(c_imeiLength, imeiLength);
                diccionarioTeltonika.Add(c_imei, imei);
                diccionarioTeltonika.Add(c_codecID, codecID);
                diccionarioTeltonika.Add(c_cantidadElementosAvlData, cantidadElementosAvlData.ToString());

                if (cantidadElementosAvlData != 0)
                {
                    const int PosicionInicioAvlData = 50;
                    string avlData = tramaTeltonika.Substring(PosicionInicioAvlData);
                    
                    int posicion = 0; //Posicion inicial a partir de la cual empiezo a obtener los ID y los valores de los IO


                    for (int contadorAvlData = 0; contadorAvlData < cantidadElementosAvlData; contadorAvlData++)
                    {
                        string c_timestamp = contadorAvlData.ToString() + ". " + "Fecha y Hora";
                        ulong timestamp = Convert.ToUInt64(avlData.Substring(posicion, 16), 16);
                        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        string dateTime = epoch.AddMilliseconds(timestamp).ToString();
                        posicion += 16;

                        string c_prioridad = contadorAvlData.ToString() + ". " + "Prioridad";
                        string prioridad = Convert.ToInt32(avlData.Substring(posicion, 2), 16).ToString();
                        posicion += 2;

                        string c_gpsLongitud = contadorAvlData.ToString() + ". " + "Longitud";
                        string gpsLongitud = (Convert.ToDouble(Convert.ToInt32(avlData.Substring(posicion, 8), 16))/10000000).ToString();
                        posicion += 8;

                        string c_gpsLatitud = contadorAvlData.ToString() + ". " + "Latitud";
                        string gpsLatitud = (Convert.ToDouble(Convert.ToInt32(avlData.Substring(posicion, 8), 16))/10000000).ToString();
                        posicion += 8;

                        string c_gpsAltura = contadorAvlData.ToString() + ". " + "Altura";
                        string gpsAltura = Convert.ToInt32(avlData.Substring(posicion, 4), 16).ToString();
                        posicion += 4;

                        string c_gpsAngulo = contadorAvlData.ToString() + ". " + "Ángulo";
                        string gpsAngulo = Convert.ToInt32(avlData.Substring(posicion, 4), 16).ToString();
                        posicion += 4;
                            
                        string c_gpsSatelites = contadorAvlData.ToString() + ". " + "Satélites";
                        string gpsSatelites = Convert.ToInt32(avlData.Substring(posicion, 2), 16).ToString();
                        posicion += 2;

                        string c_gpsVelocidad = contadorAvlData.ToString() + ". " + "Velocidad";
                        string gpsVelocidad = Convert.ToInt32(avlData.Substring(posicion, 4), 16).ToString();
                        posicion += 4;

                        diccionarioTeltonika.Add(c_timestamp, dateTime);
                        diccionarioTeltonika.Add(c_prioridad, prioridad);
                        diccionarioTeltonika.Add(c_gpsLongitud, gpsLongitud);
                        diccionarioTeltonika.Add(c_gpsLatitud, gpsLatitud);
                        diccionarioTeltonika.Add(c_gpsAltura, gpsAltura);
                        diccionarioTeltonika.Add(c_gpsAngulo, gpsAngulo);
                        diccionarioTeltonika.Add(c_gpsSatelites, gpsSatelites);
                        diccionarioTeltonika.Add(c_gpsVelocidad, gpsVelocidad);

                        string c_ioEventID = contadorAvlData.ToString() + ". " + "Event ID";
                        string ioEventID = Convert.ToInt32(avlData.Substring(posicion, 4), 16).ToString();
                        posicion += 4;

                        string c_ioCantidadElementos = contadorAvlData.ToString() + ". " + "Cantidad de Elementos I/O";
                        int ioCantidadElementos = Convert.ToInt32(avlData.Substring(posicion, 4), 16);
                        posicion += 4;

                        diccionarioTeltonika.Add(c_ioEventID, ioEventID);
                        diccionarioTeltonika.Add(c_ioCantidadElementos, ioCantidadElementos.ToString());

                        

                        if (ioCantidadElementos != 0)
                        {
                            string c_ioCantidadElementos1B = contadorAvlData.ToString() + ". " + "Elementos de 1B";
                            int ioCantidadElementos1B = Convert.ToInt32(avlData.Substring(posicion, 4), 16);
                            posicion += 4;

                            diccionarioTeltonika.Add(c_ioCantidadElementos1B, ioCantidadElementos1B.ToString());

                            if (ioCantidadElementos1B != 0)
                            {
                                for (int contadorElementos1B = 0; contadorElementos1B < ioCantidadElementos1B; contadorElementos1B++)
                                {
                                    string c_ioID = contadorAvlData.ToString() + ". " + "1B.ID" + contadorElementos1B.ToString();
                                    string ioID = Convert.ToInt32(avlData.Substring(posicion, 4), 16).ToString();
                                    posicion += 4;
                                    string c_ioValor = contadorAvlData.ToString() + ". " + "1B.Valor" + contadorElementos1B.ToString();
                                    string ioValor = Convert.ToInt32(avlData.Substring(posicion, 2), 16).ToString();
                                    posicion += 2;

                                    switch (ioID)
                                    {
                                        case "239":
                                            ioID = ioID + " - Ignición";
                                            break;
                                        case "240":
                                            ioID = ioID + " - Movimiento";
                                            break;
                                        case "80":
                                            ioID = ioID + " - Data Mode";
                                            switch (ioValor)
                                            {
                                                case "0": ioValor = ioValor + " - Home on Stop";
                                                    break;
                                                case "1":
                                                    ioValor = ioValor + " - Home on Moving";
                                                    break;
                                                case "2":
                                                    ioValor = ioValor + " - Roaming on Stop";
                                                    break;
                                                case "3":
                                                    ioValor = ioValor + " - Roaming on Moving";
                                                    break;
                                                case "4":
                                                    ioValor = ioValor + " - Unknown on Stop";
                                                    break;
                                                case "5":
                                                    ioValor = ioValor + " - Unknown on Moving";
                                                    break;
                                            }
                                            break;
                                        case "21":
                                            ioID = ioID + " - GSM Signal Strength (1 - 5)";
                                            break;
                                        case "200":
                                            ioID = ioID + " - Sleep Mode";
                                            switch (ioValor)
                                            {
                                                case "0":
                                                    ioValor = ioValor + " - No Sleep";
                                                    break;
                                                case "1":
                                                    ioValor = ioValor + " - GPS Sleep";
                                                    break;
                                                case "2":
                                                    ioValor = ioValor + " - Deep Sleep";
                                                    break;
                                                case "3":
                                                    ioValor = ioValor + " - Online Sleep";
                                                    break;
                                            }
                                            break;
                                        case "69":
                                            ioID = ioID + " - GNSS Status";
                                            switch (ioValor)
                                            {
                                                case "0":
                                                    ioValor = ioValor + " - OFF";
                                                    break;
                                                case "1":
                                                    ioValor = ioValor + " - On with Fix";
                                                    break;
                                                case "2":
                                                    ioValor = ioValor + " - On Without Fix";
                                                    break;
                                                case "3":
                                                    ioValor = ioValor + " - In sleep state";
                                                    break;
                                            }
                                            break;
                                        case "1":
                                            ioID = ioID + " - Din";
                                            break;
                                        case "179":
                                            ioID = ioID + " - Dout";
                                            break;
                                        case "10":
                                            ioID = ioID + " - SD Status";
                                            break;
                                        case "250":
                                            ioID = ioID + " - Trip Event";
                                            switch (ioValor)
                                            {
                                                case "0":
                                                    ioValor = ioValor + " - Trip Ended";
                                                    break;
                                                case "1":
                                                    ioValor = ioValor + " - Trip Started";
                                                    break;
                                                case "2":
                                                    ioValor = ioValor + " - Business Status";
                                                    break;
                                                case "3":
                                                    ioValor = ioValor + " - Private Status";
                                                    break;
                                                default:
                                                    ioValor = ioValor + " - Custom Status";
                                                    break;
                                            }
                                            break;
                                        case "255":
                                            ioID = ioID + " - Evento de Exceso de Velocidad";
                                            ioValor = ioValor + " - Velocidad en km/h";
                                            break;
                                        case "251":
                                            ioID = ioID + " - Idling Event";
                                            switch (ioValor)
                                            {
                                                case "0":
                                                    ioValor = ioValor + " - Ended";
                                                    break;
                                                case "1":
                                                    ioValor = ioValor + " - Started";
                                                    break;
                                            }
                                            break;
                                        case "253":
                                            ioID = ioID + " - Green Driving Type";
                                            switch (ioValor)
                                            {
                                                case "1":
                                                    ioValor = ioValor + " - Acceleration";
                                                    break;
                                                case "2":
                                                    ioValor = ioValor + " - Breaking";
                                                    break;
                                                case "3":
                                                    ioValor = ioValor + " - Cornering";
                                                    break;
                                            }
                                            break;
                                        case "254":
                                            ioID = ioID + " - Green Driving Value";
                                            break;
                                        case "243":
                                            ioID = ioID + " - Green Driving Event Duration";
                                            ioValor = ioValor + " ms";
                                            break;
                                        case "246":
                                            ioID = ioID + " - Towing Detection Event";
                                            break;
                                        case "252":
                                            ioID = ioID + " - Unplug Event";
                                            break;
                                        case "247":
                                            ioID = ioID + " - Crash Detection";
                                            break;
                                        case "249":
                                            ioID = ioID + " - Jamming Detection";
                                            break;
                                    }
                                    diccionarioTeltonika.Add(c_ioID, ioID);
                                    diccionarioTeltonika.Add(c_ioValor, ioValor);
                                }
                            }


                            string c_ioCantidadElementos2B = contadorAvlData.ToString() + ". " + "Cantidad de Elementos de 2B";
                            int ioCantidadElementos2B = Convert.ToInt32(avlData.Substring(posicion, 4), 16);
                            posicion += 4;

                            diccionarioTeltonika.Add(c_ioCantidadElementos2B, ioCantidadElementos2B.ToString());

                            if (ioCantidadElementos2B != 0)
                            {
                                for (int contadorElementos2B = 0; contadorElementos2B < ioCantidadElementos2B; contadorElementos2B++)
                                {
                                    string c_ioID = contadorAvlData.ToString() + ". " + "2B.ID" + contadorElementos2B.ToString();
                                    string ioID = Convert.ToInt32(avlData.Substring(posicion, 4), 16).ToString();
                                    posicion += 4;
                                    string c_ioValor = contadorAvlData.ToString() + ". " + "2B.Valor" + contadorElementos2B.ToString();
                                    string ioValor = Convert.ToInt32(avlData.Substring(posicion, 4), 16).ToString();
                                    posicion += 4;

                                    switch (ioID)
                                    {
                                        case "181":
                                            ioID = ioID + " - PDOP";
                                            ioValor = ioValor + " (Probability * 10; 0-500)";
                                            break;
                                        case "182":
                                            ioID = ioID + " - HDOP";
                                            ioValor = ioValor + " (Probability * 10; 0-500)";
                                            break;
                                        case "66":
                                            ioID = ioID + " - Ext Voltage";
                                            ioValor = ioValor + " (mv, 0 - 30 V)";
                                            break;
                                        case "24":
                                            ioID = ioID + " - Velocidad";
                                            ioValor = ioValor + " km/h";
                                            break;
                                        case "205":
                                            ioID = ioID + " - GSM Cell ID";
                                            ioValor = ioValor + " (GSM Base Station ID)";
                                            break;
                                        case "206":
                                            ioID = ioID + " - GSM Area Code";
                                            ioValor = ioValor + " Location Area Code (LAC)";
                                            break;
                                        case "67":
                                            ioID = ioID + " - Battery Voltage";
                                            ioValor = ioValor + " mV";
                                            break;
                                        case "68":
                                            ioID = ioID + " - Battery Current";
                                            ioValor = ioValor + " mA";
                                            break;
                                        case "9":
                                            ioID = ioID + " - Ain";
                                            ioValor = ioValor + " (Voltage: mV, 0 – 30 V)";
                                            break;
                                        case "13":
                                            ioID = ioID + " - Average Fuel Use";
                                            ioValor = ioValor + " - Average Fuel use in (Litersx100) /100km";
                                            break;
                                        case "17":
                                            ioID = ioID + " - Accelerometer X Axis";
                                            ioValor = ioValor + " mG; (range [-8000; 8000])";
                                            break;
                                        case "18":
                                            ioID = ioID + " - Accelerometer Y Axis";
                                            ioValor = ioValor + " mG; (range [-8000; 8000])";
                                            break;
                                        case "19":
                                            ioID = ioID + " - Accelerometer Z Axis";
                                            ioValor = ioValor + " mG; (range [-8000; 8000])";
                                            break;
                                        case "15":
                                            ioID = ioID + " - Eco Score";
                                            ioValor = ioValor + "  - Average amount of events on some distance";
                                            break;
                                    }

                                    diccionarioTeltonika.Add(c_ioID, ioID);
                                    diccionarioTeltonika.Add(c_ioValor, ioValor);
                                }
                            }

                            string c_ioCantidadElementos4B = contadorAvlData.ToString() + ". " + "Cantidad de Elementos de 4B";
                            int ioCantidadElementos4B = Convert.ToInt32(avlData.Substring(posicion, 4), 16);
                            posicion += 4;

                            diccionarioTeltonika.Add(c_ioCantidadElementos4B, ioCantidadElementos4B.ToString());

                            if (ioCantidadElementos4B != 0)
                            {
                                for (int contadorElementos4B = 0; contadorElementos4B < ioCantidadElementos4B; contadorElementos4B++)
                                {
                                    string c_ioID = contadorAvlData.ToString() + ". " + "4B.ID" + contadorElementos4B.ToString();
                                    string ioID = Convert.ToInt32(avlData.Substring(posicion, 4), 16).ToString();
                                    posicion += 4;
                                    string c_ioValor = contadorAvlData.ToString() + ". " + "4B.Valor" + contadorElementos4B.ToString();
                                    string ioValor = Convert.ToUInt32(avlData.Substring(posicion, 8), 16).ToString();
                                    posicion += 8;

                                    switch (ioID)
                                    {
                                        case "241":
                                            ioID = ioID + " - GSM Operator";;
                                            break;
                                        case "199":
                                            ioID = ioID + " - Trip Odometer";
                                            ioValor = ioValor + " m";
                                            break;
                                        case "16":
                                            ioID = ioID + " - Total Odometer";
                                            ioValor = ioValor + " m";
                                            break;
                                        case "19":
                                            ioID = ioID + " - Fuel Used GPS";
                                            ioValor = ioValor + " mL";
                                            break;
                                    }

                                    diccionarioTeltonika.Add(c_ioID, ioID);
                                    diccionarioTeltonika.Add(c_ioValor, ioValor);
                                }
                            }

                            string c_ioCantidadElementos8B = contadorAvlData.ToString() + ". " + "Elementos de 8B";
                            int ioCantidadElementos8B = Convert.ToInt32(avlData.Substring(posicion, 4), 16);
                            posicion += 4;

                            diccionarioTeltonika.Add(c_ioCantidadElementos8B, ioCantidadElementos8B.ToString());

                            if (ioCantidadElementos8B != 0)
                            {
                                for (int contadorElementos8B = 0; contadorElementos8B < ioCantidadElementos8B; contadorElementos8B++)
                                {
                                    string c_ioID = contadorAvlData.ToString() + ". " + "8B.ID" + contadorElementos8B.ToString();
                                    string ioID = Convert.ToInt32(avlData.Substring(posicion, 4), 16).ToString();
                                    posicion += 4;
                                    string c_ioValor = contadorAvlData.ToString() + ". " + "8B.Valor" + contadorElementos8B.ToString();
                                    string ioValor = Convert.ToUInt64(avlData.Substring(posicion, 16), 16).ToString();
                                    posicion += 16;

                                    switch(ioID)
                                    {
                                        case "11":
                                            ioID = ioID + " - SIM ICCID number part 1";
                                            break;
                                        case "14":
                                            ioID = ioID + " - SIM ICCID number part 2";
                                            break;
                                        case "238":
                                            ioID = ioID + " - User ID (MAC address of NMEA receiver device connected via Bluetooth)";
                                            break;
                                    }

                                    diccionarioTeltonika.Add(c_ioID, ioID);
                                    diccionarioTeltonika.Add(c_ioValor, ioValor);
                                }
                            }

                            string c_ioCantidadElementosXB = contadorAvlData.ToString() + ". " + "Elementos de XB";
                            int ioCantidadElementosXB = Convert.ToInt32(avlData.Substring(posicion, 4), 16);
                            posicion += 4;

                            diccionarioTeltonika.Add(c_ioCantidadElementosXB, ioCantidadElementosXB.ToString());

                            if (ioCantidadElementosXB != 0)
                            {
                                for (int contadorElementosXB = 0; contadorElementosXB < ioCantidadElementosXB; contadorElementosXB++)
                                {
                                    string c_ioID = contadorAvlData.ToString() + ". " + "XB.ID" + contadorElementosXB.ToString();
                                    string ioID = Convert.ToInt32(avlData.Substring(posicion, 4), 16).ToString();
                                    posicion += 4;
                                    string c_ioLength = contadorAvlData.ToString() + ". " + "XB.Length" + contadorElementosXB.ToString();
                                    int ioLength = Convert.ToInt32(avlData.Substring(posicion, 4), 16);
                                    posicion += 4;

                                    string c_ioValor = contadorAvlData.ToString() + ". " + "XB.Valor" + contadorElementosXB.ToString();
                                    string ioValor = Convert.ToUInt64(avlData.Substring(posicion, ioLength * 2), 16).ToString();
                                    posicion += ioLength * 2;
                                    diccionarioTeltonika.Add(c_ioID, ioID);
                                    diccionarioTeltonika.Add(c_ioValor, ioValor);
                                }
                            }
                        }
                    }
                }
                return diccionarioTeltonika;
            }
            else return diccionarioError;
        }
    }
}
