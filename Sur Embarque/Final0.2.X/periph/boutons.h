#ifndef _BOUTONS_H_
#define _BOUTONS_H_

#define BT_DO	PORTEbits.RE0           //I/O pour le bouton bas
#define BT_LE	PORTEbits.RE1           //I/O pour le bouton gauche
#define BT_EN	PORTEbits.RE2           //I/O pour le bouton centre
#define BT_RI	PORTCbits.RC0           //I/O pour le bouton droit
#define BT_UP	PORTCbits.RC1           //I/O pour le bouton haut
#define INT_BT  INTCONbits.INT0IF       //drapeau d'interruption pour INT0

//**************************************************
//* PROROTYPES POUR INITALISER L'ETAT DES BOUTTONS *
//**************************************************
void initBout(void);            //définit les tris et les interruptions pour le bouttons

		//placer initBout(); à  la config des pins
		
#endif