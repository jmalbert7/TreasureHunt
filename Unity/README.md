# TreasureHunt
CS467 Capstone Project for Jessica Albert, Louis Adams, Logan Cope

Deploying to Azure
	1. Build the Unity game w/ WebGL, no compression, no decompression fallback. Save to webGLBuild folder.
	2. Open your favorite file transfer software (FileZilla, WinSCP)
	3. Navigate to the Azure Portal page then to the App Service (TreasureHuntGroupX)
	4. Copy the FTPS host name. Enter it into your file transfer software in the 'host' section.
	5. Navigate to the 'Sources' Google Drive document to grab the FTPS username and password.
	   Enter it into the FTP software. Connect to the App Service.
	6. Make sure you are in the site/wwwroot directory of the App Service
	7. Copy local contents of the webGLBuild folder to the App Service.
	8. Open the site URL.




