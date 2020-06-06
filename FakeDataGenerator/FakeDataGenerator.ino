
// DEADBEEF|A-B-C-D-E-F-G-H + null
const uint32_t MESSAGE_SIZE = 24;
char buffer[MESSAGE_SIZE] = { 0 };

uint8_t fakeData[8] = {  
   0x00, 0x00, 0x0A, 0x40,
   0x00, 0x00, 0x0D, 0xCC
};

void shuffleFakeData() {
    fakeData[2] = random(0x9, 0xB);
    fakeData[3] = random(0x39, 0x41);
    fakeData[6] = random(0xC, 0xE);
    fakeData[7] = random(0xCB, 0xCD);
}

uint32_t fakeIdentifier() {
  return random(0xDEADBEEA, 0xDEADBEEF);
}

void setup() {
  pinMode(LED_BUILTIN, OUTPUT);
  Serial.begin(9600, SERIAL_8N1);
}

void loop() {
  delay(500);
  digitalWrite(LED_BUILTIN, HIGH);

  shuffleFakeData();

  sprintf(
    buffer, 
    "%08lX|%02X-%02X-%02X-%02X-%02X-%02X-%02X-%02X",
    fakeIdentifier(),
    fakeData[0],
    fakeData[1],
    fakeData[2],
    fakeData[3],
    fakeData[4],
    fakeData[5],
    fakeData[6],
    fakeData[7]);

  Serial.println(buffer);

  delay(500);
  digitalWrite(LED_BUILTIN, LOW);
}
