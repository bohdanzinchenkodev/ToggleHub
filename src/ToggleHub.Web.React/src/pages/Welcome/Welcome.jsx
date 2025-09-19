import React from 'react';
import switchOff from "../../assets/switch_off.png";
import switchOn from "../../assets/switch_on.png";
import rumblingOff from "../../assets/rumbling_off.png";
import rumblingOn from "../../assets/rumbling_on.jpg";
import {useState} from "react";
import {Button, Box, Typography, Grid, IconButton, Container} from "@mui/material";
import {Link} from "react-router";

const Welcome = () => {
	const [switchedOn, setSwitchedOn] = useState(false);

	const switchImage = switchedOn ? switchOn : switchOff;
	const rumblingImage = switchedOn ? rumblingOn : rumblingOff;
	  return (
		  <Container maxWidth="md">
			  <Box
				  sx={{
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

				  <Grid container sx={{ alignItems: 'end' }}>
					  {/* Sign up and Sign in buttons - left side on desktop, first on mobile */}
					  <Grid item size={{xs: 12, md: 4}} >
						  <Box sx={{ display: 'flex', justifyContent: 'center' }}>
							  <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' , justifyContent: 'space-between' }}>
								  <IconButton
									  onClick={() => setSwitchedOn(!switchedOn)}
									  disableRipple
									  sx={{
										  p: 1,
										  '&:hover': {
											  backgroundColor: 'transparent'
										  },
										  '&:active': {
											  backgroundColor: 'transparent'
										  },
										  '&:focus': {
											  backgroundColor: 'transparent'
										  }
									  }}
								  >
									  <Box
										  component="img"
										  src={switchImage}
										  alt="rumbling switch"
										  sx={{ maxWidth: '96px', height: 'auto' }}
									  />
								  </IconButton>

								  <Link to="/login" style={{ textDecoration: 'none' }}>
									  <Button
										  variant="outlined"
										  sx={{ width: '200px', height: '48px', mb: 2 }}
									  >
										  Sign in
									  </Button>
								  </Link>
								  <Link to="/register" style={{ textDecoration: 'none' }}>
									  <Button
										  variant="contained"
										  sx={{ width: '200px', height: '48px' }}
									  >
										  Sign up
									  </Button>
								  </Link>
							  </Box>
						  </Box>
					  </Grid>

					  {/* Image and switch section - right side on desktop, second on mobile */}
					  <Grid item size={{xs: 12, md: 8}} >
						  <Box sx={{ position: 'relative', display: 'flex', alignItems: 'center', justifyContent: 'start' }}>
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
						  </Box>
					  </Grid>
				  </Grid>
			  </Box>
		  </Container>
	  );
}
export default Welcome;
