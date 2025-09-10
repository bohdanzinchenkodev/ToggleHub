import React from 'react';
import switchOff from "../../assets/switch_off.png";
import switchOn from "../../assets/switch_on.png";
import rumblingOff from "../../assets/rumbling_off.png";
import rumblingOn from "../../assets/rumbling_on.jpg";
import {useState} from "react";
import {Button} from "@mui/material";

const Welcome = () => {
	const [switchedOn, setSwitchedOn] = useState(false);

	const switchImage = switchedOn ? switchOn : switchOff;
	const rumblingImage = switchedOn ? rumblingOn : rumblingOff;
	  return (
		  <div className="">
			  <h1 className="text-4xl font-bold mb-4 text-center">Welcome To ToggleHub</h1>
			  <p className="text-lg mb-20 text-center">You could even toggle the Rumbling.</p>

			  <div className="grid grid-cols-2 gap-8 items-center">
				  {/* Left side - Sign up and Sign in buttons */}

				  {/* Right side - Image and switch */}
				  <div className="relative flex items-center justify-center">
					  {/* Switch button positioned to the left of image */}
					  <button
						  type="button"
						  className="cursor-pointer absolute left-0 transform -translate-x-32 flex items-center justify-center p-2"
						  onClick={() => setSwitchedOn(!switchedOn)}
					  >
						  <img src={switchImage} alt="rumbling switch" className="max-w-24" />
					  </button>


					  <div className="relative flex flex-col items-center w-96">
						  {/* Lamp fixture
						  <div className="absolute left-1/2 -translate-x-1/2 w-36 h-6 rounded-md bg-gray-900 shadow-lg z-30" />

						   Gradient light glow
						  <div
							  className={`absolute z-20 top-8 left-1/2 -translate-x-1/2 w-72 h-72 rounded-full pointer-events-none ease-in-out ${
								  switchedOn ? "opacity-60 scale-100" : "opacity-0 scale-75"
							  }`}
							  style={{
								  background: "radial-gradient(circle, rgba(255,255,200,0.8) 0%, rgba(255,255,200,0.05) 70%, transparent 100%)",
							  }}
						  />*/}


						  {/* Main image */}
						  <img
							  src={rumblingImage}
							  alt="toggle image"
							  className={`relative z-10 max-w-full max-h-full object-contain mt-3  ${
								  switchedOn
									  ? "brightness-125 contrast-110 saturate-110"
									  : "brightness-50 contrast-90 saturate-75"
							  }`}
						  />
					  </div>
				  </div>
				  <div className="flex flex-col items-center justify-center">
					  <Button variant="outlined" sx={{ width: '200px', height: '48px', mb: 2 }}>Sign in</Button>
					  <Button variant="contained" sx={{ width: '200px', height: '48px' }}>Sign up</Button>
				  </div>
			  </div>
		  </div>
	  );
}
export default Welcome;
