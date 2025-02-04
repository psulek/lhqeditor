declare namespace amb {
	export interface IXStrings {
		Cars: {
			Diesel: {
				Old: {
					Old_Kia: string;
				},
				Mercedes: string;
				Skoda: string;
			},
			Electric: {
				ToyotaEV: string;
			}
		},
		Messages: {
			Empty: string;
			Title: string;
		},
		Welcome: string;
	}
	
	export interface IXStringsMetadata {
	    default: string;
	    languages: Array<string>;
	}
	
}
