#ifndef _HEAD_H_
#define _HEAD_H_

#include <p18lf46k22.h>
#include <delays.h>
#include "periph/affi.h"
#include "periph/boutons.h"
#include "periph/usart.h"
#include "periph/1wire.h"
#include "ftoa/ftoa.h"
#include "xlcd.h"
#include "TIMERS.h"
#include "TIOS.h"
#include "usart.h"

//**********************
//* VARIABLES GLOBALES *
//**********************
enum {NUM, ANA};        //NUM = 0, ANA = 1
enum {OUT, IN};         //OUT = 0, IN = 1
enum {OFF, ON};         //OFF = 0, ON = 1

//**********
//* DEFINE *
//**********
#define IO_LED  PORTCbits.RC2       //I/O pour la led
#define IO_REL  PORTBbits.RB4       //I/O pour le relais

#endif