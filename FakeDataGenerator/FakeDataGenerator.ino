
// DEADBEEF|ABCD-EFGH + null
const uint32_t MESSAGE_SIZE = 18;
char buffer[MESSAGE_SIZE] = { 0 };

uint8_t fakeData[8] = {  
   0x00, 0x00, 0x0A, 0x40,
   0x00, 0x00, 0x0D, 0xCC
};

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

  sprintf(
    buffer, 
    "%08lX|%02X%02X%02X%02X-%02X%02X%02X%02X",
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
