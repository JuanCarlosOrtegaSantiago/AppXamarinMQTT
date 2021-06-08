/*
 * Comandos: 
 * L1->Enceder led
 * L0->Apagar Led
 * ?L->Envia el estado del led
 * ?F->Enviar luminosidad
 * S090->Mover el servo a 90°
 * S180->Mover el servo a 180°
 * S000->Mover el servo a 0°
 * ?S->Envia el angulo actual del servomotor
 */


#include <Servo.h>
#include <ESP8266WiFi.h>
#include <PubSubClient.h>
const char *ssid = "TP-Link_F530";
const char *password = "77360090";
const char* mqtt_server = "broker.hivemq.com";

Servo servo;
#define pinServo D6
#define pinLed D2
#define pinLDR D0

String comando="";
int estadoLed=0;
int anguloServo=0;

WiFiClient clientMqtt;
PubSubClient mqtt(clientMqtt);


void setup() {
  Serial.begin(9600);
  pinMode(pinLed,OUTPUT);
  pinMode(pinLDR,INPUT);
  servo.attach(D6);
  digitalWrite(pinLed,LOW);
  servo.write(0);
  ConectarWiFi();
  
}

void ConectarWiFi(){
    WiFi.begin(ssid,password);
    Serial.print("Conectando a SSID: ");
    Serial.println(ssid);
    while(WiFi.status()!=WL_CONNECTED){
      Serial.print(".");
      delay(500);
    }
    Serial.print("Conexion Establecida con ");
    Serial.println(ssid);
    Serial.print(" con la IP:");
    Serial.println(WiFi.localIP());
    Serial.print("Conectando con MQTT:");
    mqtt.setServer(mqtt_server, 1883);
    mqtt.connect("ESP8266ProfeCarlos");
    while(!mqtt.connected()){
      Serial.print(".");
      delay(500);
    }
    Serial.println("Conectado a MQTT :)");
    mqtt.setCallback(callback);
    mqtt.subscribe("ESP8266ProfeCarlos");
    mqtt.publish("ServerProfeCarlos","Conectado");
}

void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Mensaje MQTT [");
  Serial.print(topic);
  Serial.print("] ");
  comando="";
  for (int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
    comando+=String((char)payload[i]);
  }
  Serial.println();
}


void loop() {
  mqtt.loop();
  VerficarComando();
  if(comando!=""){
    if(comando=="L1"){
      digitalWrite(pinLed,HIGH);
      estadoLed=1;
      Imprime("LED ENCENDIDO");
    }
    if(comando=="L0"){
      digitalWrite(pinLed,LOW);
      estadoLed=0;
      Imprime("LED APAGADO");
    }
    if(comando=="?L"){
      if(estadoLed==0){
        Imprime("LED APAGADO");
      }else{
        Imprime("LED ENCENDIDO");
      }
    }
    if(comando=="?F"){
      Imprime("Fotoresistencia=" + String(analogRead(pinLDR)));
    }
    if(comando=="?S"){
      Imprime("Servo=" + String(anguloServo));
    }
    if(comando[0]=='S'){
      //comando de servo, ejemplo S090
      char buf[4];
      comando.substring(1).toCharArray(buf,4);
      anguloServo=atoi(buf);
      servo.write(anguloServo);
      Imprime("Servo=" + String(anguloServo));
    }
    comando="";
  }

}

void Imprime(String mensaje)
{
  Serial.println(mensaje);
  char buf[50];
  mensaje.toCharArray(buf,50);
  mqtt.publish("ServerProfeCarlos",buf);
}

void VerficarComando(){
  if(Serial.available()){
    comando=Serial.readStringUntil('\n');
  }
}
