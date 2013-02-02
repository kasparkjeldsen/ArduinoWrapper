const int chars = 8;
char out[chars];
int a = 0;
bool handshake = false;
void StreamPrint_progmem(Print &out,PGM_P format,...) //http://www.utopiamechanicus.com/article/low-memory-serial-print/
{
  // program memory version of printf - copy of format string and result share a buffer
  // so as to avoid too much memory use
  char formatString[128], *ptr;
  strncpy_P( formatString, format, sizeof(formatString) ); // copy in from program mem
  // null terminate - leave last char since we might need it in worst case for result's \0
  formatString[ sizeof(formatString)-2 ]='\0'; 
  ptr=&formatString[ strlen(formatString)+1 ]; // our result buffer...
  va_list args;
  va_start (args,format);
  vsnprintf(ptr, sizeof(formatString)-1-strlen(formatString), formatString, args );
  va_end (args);
  formatString[ sizeof(formatString)-1 ]='\0'; 
  out.print(ptr);
}
 
#define Serialprint(format, ...) StreamPrint_progmem(Serial,PSTR(format),##__VA_ARGS__)
#define Streamprint(stream,format, ...) StreamPrint_progmem(stream,PSTR(format),##__VA_ARGS__)

void setup() {
	Serial.begin(115200); // Initialize serial port
    pinMode(13,OUTPUT);
    digitalWrite(13,HIGH);                     
}
void loop()
{
	read();
	if(!handshake) shake();
	if(handshake) handle();
	output();
}
void shake()
{
	char c = out[1];
	if(c == 72)
	{
		handshake = true;
		Serial.println("?aCK;*");
	}
	else handshake = false;
}
void handle()
{
	if(a > chars-1)
	{
		if(out[0] == 91 && out[chars-1] == 93) //check den er format "[...]"
		{
			char C = out[1];
			switch(C)
			{
                        case 65:
                            analogOut(out);
                        break;
			case 100: //d digital pin
				digitalOut(out);
				break;
			case 83: //setup
				runSetup(out);
				break;
			case 105: //read digital pin
				digitalRead(out);
				break;
			}
		}
	}
}
void analogOut(char s[])
{
        int port = charToInt(s[2]);
	if(s[3] != 59) port = (port*10) + charToInt(s[3]);
	int result = analogRead(port);
	Serialprint("?A%d;ACK;RETURN:%d;*\n",port,result);
}
void digitalRead(char s[])
{
	int port = charToInt(s[2]);
	if(s[3] != 59) port = (port*10) + charToInt(s[3]);
	int result = digitalRead(port);
	Serialprint("?i%d;ACK;RETURN:%d;*\n",port,result);
}
void runSetup(char s[])
{
	char C = s[2];
	switch(C)
	{
	case 100: //d
		int port = charToInt(s[4]);
		if(s[5] != 59) port = (port*10) + charToInt(s[5]);
		int start = 3;
		int level = charToInt(s[3]);
		if(level == 0) pinMode(port,INPUT);
		else pinMode(port,OUTPUT);
		Serialprint("?Sd%d%d;ACK;*\n",level,port);
		break;
	}
}
void digitalOut(char s[])
{
	int port = charToInt(s[3]);
	if(s[4] != 59) port = (port*10) + charToInt(s[4]);
	Serialprint("d%\n",s[2]);
	int level = charToInt(s[2]);
	if(level == 1) level = HIGH;
	else level = LOW;
	digitalWrite(port,level);
	Serialprint("?d%d%d;ACK;*\n",level,port);
}
int charToInt(char c)
{
	char * t = &c;
	return atoi(t);
}
void output()
{
	if(a > chars-1)
	{
		a = 0;
		for(int i = 0; i < chars; i++) out[i] = NULL;
	}
}
void read()
{
	while(Serial.available() > 0)
	{
		char c = Serial.read();
		out[a] = c;
		a++;
		if(a == chars) break;
	}
	char c = out[1];
	if(c == 72)
	{
		handshake = true;
		Serial.println("?aCK;*");
	}
}


