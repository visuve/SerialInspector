
// DEADBEEF|AA-BB-CC-DD-EE-FF-GG-HH + newline + null
const uint32_t MESSAGE_SIZE = 35;
char buffer[MESSAGE_SIZE] = { 0 };

uint8_t fakeData[8] = {  
   0x00, 0x00, 0x0A, 0x40,
   0x00, 0x00, 0x0D, 0xCC
};

void shuffleFakeData() {
    fakeData[2] = random(0x9, 0xB);
    fakeData[3] = random(0x38, 0x42);
    fakeData[6] = random(0xC, 0xE);
    fakeData[7] = random(0xCA, 0xCE);
}

uint32_t fakeIdentifier() {
  return random(0xDEADBEAA, 0xDEADBEFF);
}

void setup() {
  pinMode(LED_BUILTIN, OUTPUT);
  Serial.begin(115200, SERIAL_8N1);
}

void loop() {
  digitalWrite(LED_BUILTIN, HIGH);
  delay(500);

  shuffleFakeData();

  sprintf(
    buffer, 
    "%08lX|%02X-%02X-%02X-%02X-%02X-%02X-%02X-%02X\r\n",
    fakeIdentifier(),
    fakeData[0],
    fakeData[1],
    fakeData[2],
    fakeData[3],
    fakeData[4],
    fakeData[5],
    fakeData[6],
    fakeData[7]);

  Serial.write(buffer, MESSAGE_SIZE - 1);

  digitalWrite(LED_BUILTIN, LOW);
  delay(500);
}
