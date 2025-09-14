import React from 'react';
import switchOff from "../../assets/switch_off.png";
import switchOn from "../../assets/switch_on.png";
import rumblingOff from "../../assets/rumbling_off.png";
import rumblingOn from "../../assets/rumbling_on.jpg";
import {useState} from "react";
import {Button, Box, Typography, Grid, IconButton} from "@mui/material";
import {useNavigate} from "react-router";

const Welcome = () => {
	const [switchedOn, setSwitchedOn] = useState(false);
	const navigate = useNavigate();

	const switchImage = switchedOn ? switchOn : switchOff;
	const rumblingImage = switchedOn ? rumblingOn : rumblingOff;
	  return (
		  <Box
			  sx={{
				  display: 'flex',
				  flexDirection: 'column',
				  alignItems: 'center',
				  justifyContent: 'center',
				  p: 4,
				  minHeight: 'calc(100vh - 80px)'
			  }}
		  >
			  <Typography
				  variant="h2"
				  component="h1"
				  sx={{
					  fontWeight: 'bold',
					  mb: 2,
					  textAlign: 'center',
					  fontSize: { xs: '2rem', md: '3rem' }
				  }}
			  >
				  Welcome To ToggleHub
			  </Typography>

			  <Typography
				  variant="h6"
				  sx={{
					  mb: 5,
					  textAlign: 'center',
					  fontSize: { xs: '1rem', md: '1.25rem' }
				  }}
			  >
				  You could even toggle the Rumbling.
			  </Typography>

			  <Grid container spacing={4} sx={{ alignItems: 'center' }}>
				  {/* Sign up and Sign in buttons - left side on desktop, first on mobile */}
				  <Grid item xs={12} md={6} sx={{ order: { xs: 1, md: 1 } }}>
					  <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
						  <Button
							  onClick={() => navigate("/login")}
							  variant="outlined"
							  sx={{ width: '200px', height: '48px', mb: 2 }}
						  >
							  Sign in
						  </Button>
						  <Button
							  onClick={() => navigate("/register")}
							  variant="contained"
							  sx={{ width: '200px', height: '48px' }}
						  >
							  Sign up
						  </Button>
					  </Box>
				  </Grid>

				  {/* Image and switch section - right side on desktop, second on mobile */}
				  <Grid item xs={12} md={6} sx={{ order: { xs: 2, md: 2 } }}>
					  <Box sx={{ position: 'relative', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
						  {/* Switch button positioned to the left of image */}

						  <Box sx={{ position: 'relative', display: 'flex', flexDirection: 'column', alignItems: 'center', width: '384px' }}>
							  <Box
								  component="img"
								  src={rumblingImage}
								  alt="toggle image"
								  sx={{
									  position: 'relative',
									  zIndex: 10,
									  maxWidth: '100%',
									  maxHeight: '100%',
									  objectFit: 'contain',
									  mt: 1,
									  filter: switchedOn
										  ? 'brightness(1.25) contrast(1.1) saturate(1.1)'
										  : 'brightness(0.5) contrast(0.9) saturate(0.75)'
								  }}
							  />
						  </Box>
						  <IconButton
							  onClick={() => setSwitchedOn(!switchedOn)}
							  sx={{
								  position: 'absolute',
								  right: 0,
								  transform: 'translateX(8rem)',
								  p: 1
							  }}
						  >
							  <Box
								  component="img"
								  src={switchImage}
								  alt="rumbling switch"
								  sx={{ maxWidth: '96px', height: 'auto' }}
							  />
						  </IconButton>
					  </Box>
				  </Grid>
			  </Grid>
		  </Box>
	  );
}
export default Welcome;
