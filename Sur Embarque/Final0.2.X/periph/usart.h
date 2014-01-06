#ifndef USART_H
#define	USART_H

void initRFID(void);

#define INT_RFID    PIR3bits.RC2IF      //drapeau d'interruption pour l'usart2 (rfid)
#define INT_USART   PIR1bits.RC1IF      //drapeau d'interruption pour l'usart1 (pc)


#endif

