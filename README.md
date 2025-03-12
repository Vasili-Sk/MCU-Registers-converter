Converts .svd file and adds data into .h file, including description of bits. 
Note: there is no matching functionality yet, if registers names are different then in SVD then you would need to fix them manually

Result example:
```
typedef struct
{
	union {
		__IO uint32_t SR; /*!< ADC status register,                         Address offset: 0x00 */
		struct {
			__IO uint32_t AWD :1; /* Analog watchdog flag */
			__IO uint32_t EOC :1; /* Regular channel end of conversion */
			__IO uint32_t JEOC :1; /* Injected channel end of conversion */
			__IO uint32_t JSTRT :1; /* Injected channel start flag */
			__IO uint32_t STRT :1; /* Regular channel start flag */
			__IO uint32_t OVR :1; /* Overrun */
		}SRbits;
	};
	union {
		__IO uint32_t CR1; /*!< ADC control register 1,                      Address offset: 0x04 */
		struct {
			__IO uint32_t AWDCH :5; /* Analog watchdog channel select bits */
			__IO uint32_t EOCIE :1; /* Interrupt enable for EOC */
			__IO uint32_t AWDIE :1; /* Analog watchdog interrupt enable */
			__IO uint32_t JEOCIE :1; /* Interrupt enable for injected channels */
			__IO uint32_t SCAN :1; /* Scan mode */
			__IO uint32_t AWDSGL :1; /* Enable the watchdog on a single channel in scan mode */
			__IO uint32_t JAUTO :1; /* Automatic injected group conversion */
			__IO uint32_t DISCEN :1; /* Discontinuous mode on regular channels */
			__IO uint32_t JDISCEN :1; /* Discontinuous mode on injected channels */
			__IO uint32_t DISCNUM :3; /* Discontinuous mode channel count */
			__IO uint32_t reserved0 :6;
			__IO uint32_t JAWDEN :1; /* Analog watchdog enable on injected channels */
			__IO uint32_t AWDEN :1; /* Analog watchdog enable on regular channels */
			__IO uint32_t RES :2; /* Resolution */
			__IO uint32_t OVRIE :1; /* Overrun interrupt enable */
		}CR1bits;
	};
	union {
		__IO uint32_t CR2; /*!< ADC control register 2,                      Address offset: 0x08 */
		struct {
			__IO uint32_t ADON :1; /* A/D Converter ON / OFF */
			__IO uint32_t CONT :1; /* Continuous conversion */
			__IO uint32_t reserved0 :6;
			__IO uint32_t DMA :1; /* Direct memory access mode (for single ADC mode) */
			__IO uint32_t DDS :1; /* DMA disable selection (for single ADC mode) */
			__IO uint32_t EOCS :1; /* End of conversion selection */
			__IO uint32_t ALIGN :1; /* Data alignment */
			__IO uint32_t reserved1 :4;
			__IO uint32_t JEXTSEL :4; /* External event select for injected group */
			__IO uint32_t JEXTEN :2; /* External trigger enable for injected channels */
			__IO uint32_t JSWSTART :1; /* Start conversion of injected channels */
			__IO uint32_t reserved2 :1;
			__IO uint32_t EXTSEL :4; /* External event select for regular group */
			__IO uint32_t EXTEN :2; /* External trigger enable for regular channels */
			__IO uint32_t SWSTART :1; /* Start conversion of regular channels */
		}CR2bits;
	};
	union {
		__IO uint32_t SMPR1; /*!< ADC sample time register 1,                  Address offset: 0x0C */
		struct {
			__IO uint32_t SMPx_x :32; /* Sample time bits */
		}SMPR1bits;
	};
	union {
		__IO uint32_t SMPR2; /*!< ADC sample time register 2,                  Address offset: 0x10 */
		struct {
			__IO uint32_t SMPx_x :32; /* Sample time bits */
		}SMPR2bits;
	};
	union {
		__IO uint32_t JOFR1; /*!< ADC injected channel data offset register 1, Address offset: 0x14 */
		struct {
			__IO uint32_t JOFFSET1 :12; /* Data offset for injected channel x */
		}JOFR1bits;
	};
	union {
		__IO uint32_t JOFR2; /*!< ADC injected channel data offset register 2, Address offset: 0x18 */
		struct {
			__IO uint32_t JOFFSET2 :12; /* Data offset for injected channel x */
		}JOFR2bits;
	};
	union {
		__IO uint32_t JOFR3; /*!< ADC injected channel data offset register 3, Address offset: 0x1C */
		struct {
			__IO uint32_t JOFFSET3 :12; /* Data offset for injected channel x */
		}JOFR3bits;
	};
	union {
		__IO uint32_t JOFR4; /*!< ADC injected channel data offset register 4, Address offset: 0x20 */
		struct {
			__IO uint32_t JOFFSET4 :12; /* Data offset for injected channel x */
		}JOFR4bits;
	};
	union {
		__IO uint32_t HTR; /*!< ADC watchdog higher threshold register,      Address offset: 0x24 */
		struct {
			__IO uint32_t HT :12; /* Analog watchdog higher threshold */
		}HTRbits;
	};
	union {
		__IO uint32_t LTR; /*!< ADC watchdog lower threshold register,       Address offset: 0x28 */
		struct {
			__IO uint32_t LT :12; /* Analog watchdog lower threshold */
		}LTRbits;
	};
	union {
		__IO uint32_t SQR1; /*!< ADC regular sequence register 1,             Address offset: 0x2C */
		struct {
			__IO uint32_t SQ13 :5; /* 13th conversion in regular sequence */
			__IO uint32_t SQ14 :5; /* 14th conversion in regular sequence */
			__IO uint32_t SQ15 :5; /* 15th conversion in regular sequence */
			__IO uint32_t SQ16 :5; /* 16th conversion in regular sequence */
			__IO uint32_t L :4; /* Regular channel sequence length */
		}SQR1bits;
	};
	union {
		__IO uint32_t SQR2; /*!< ADC regular sequence register 2,             Address offset: 0x30 */
		struct {
			__IO uint32_t SQ7 :5; /* 7th conversion in regular sequence */
			__IO uint32_t SQ8 :5; /* 8th conversion in regular sequence */
			__IO uint32_t SQ9 :5; /* 9th conversion in regular sequence */
			__IO uint32_t SQ10 :5; /* 10th conversion in regular sequence */
			__IO uint32_t SQ11 :5; /* 11th conversion in regular sequence */
			__IO uint32_t SQ12 :5; /* 12th conversion in regular sequence */
		}SQR2bits;
	};
	union {
		__IO uint32_t SQR3; /*!< ADC regular sequence register 3,             Address offset: 0x34 */
		struct {
			__IO uint32_t SQ1 :5; /* 1st conversion in regular sequence */
			__IO uint32_t SQ2 :5; /* 2nd conversion in regular sequence */
			__IO uint32_t SQ3 :5; /* 3rd conversion in regular sequence */
			__IO uint32_t SQ4 :5; /* 4th conversion in regular sequence */
			__IO uint32_t SQ5 :5; /* 5th conversion in regular sequence */
			__IO uint32_t SQ6 :5; /* 6th conversion in regular sequence */
		}SQR3bits;
	};
	union {
		__IO uint32_t JSQR; /*!< ADC injected sequence register,              Address offset: 0x38*/
		struct {
			__IO uint32_t JSQ1 :5; /* 1st conversion in injected sequence */
			__IO uint32_t JSQ2 :5; /* 2nd conversion in injected sequence */
			__IO uint32_t JSQ3 :5; /* 3rd conversion in injected sequence */
			__IO uint32_t JSQ4 :5; /* 4th conversion in injected sequence */
			__IO uint32_t JL :2; /* Injected sequence length */
		}JSQRbits;
	};
	union {
		__IO uint32_t JDR1; /*!< ADC injected data register 1,                Address offset: 0x3C */
		struct {
			__IO uint32_t JDATA :16; /* Injected data */
		}JDR1bits;
	};
	union {
		__IO uint32_t JDR2; /*!< ADC injected data register 2,                Address offset: 0x40 */
		struct {
			__IO uint32_t JDATA :16; /* Injected data */
		}JDR2bits;
	};
	union {
		__IO uint32_t JDR3; /*!< ADC injected data register 3,                Address offset: 0x44 */
		struct {
			__IO uint32_t JDATA :16; /* Injected data */
		}JDR3bits;
	};
	union {
		__IO uint32_t JDR4; /*!< ADC injected data register 4,                Address offset: 0x48 */
		struct {
			__IO uint32_t JDATA :16; /* Injected data */
		}JDR4bits;
	};
	union {
		__IO uint32_t DR; /*!< ADC regular data register,                   Address offset: 0x4C */
		struct {
			__IO uint32_t DATA :16; /* Regular data */
		}DRbits;
	};
}ADC_TypeDef;
```
