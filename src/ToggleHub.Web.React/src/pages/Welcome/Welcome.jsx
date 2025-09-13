import React from 'react';
import switchOff from "../../assets/switch_off.png";
import switchOn from "../../assets/switch_on.png";
import rumblingOff from "../../assets/rumbling_off.png";
import rumblingOn from "../../assets/rumbling_on.jpg";
import {useState} from "react";
import {Button} from "@mui/material";
import {useNavigate} from "react-router";

const Welcome = () => {
	const [switchedOn, setSwitchedOn] = useState(false);
	const navigate = useNavigate();

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
					  <Button onClick={() => navigate("/login")} variant="outlined" sx={{ width: '200px', height: '48px', mb: 2 }}>Sign in</Button>
					  <Button onClick={() => navigate("/register")} variant="contained" sx={{ width: '200px', height: '48px' }}>Sign up</Button>
				  </div>
			  </div>
		  </div>
	  );
}
export default Welcome;
