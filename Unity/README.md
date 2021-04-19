# TreasureHunt
CS467 Capstone Project for Jessica Albert, Louis Adams, Logan Cope

Deploying to Azure
	1. In Unity, navigate to File -> Build Settings, make sure WebGL is selected in the Platform pane. Click on Build
		a. A file explorer window will pop up, navigate to Unity -> webGLBuild. Click Select Folder
		b. the build will take several minutes
	2. Navigate to Unity -> webGLBuild -> TreasureHuntDeploy open TreasureHuntDeploy.sln
		a. Navigate to File -> Open -> Website
		b. Open the Build directory within the webGLBuild folder
		c. Navigate to Build -> Publish Website
		d. select Azure -> Azure App Service
		e. Select CS467_groupx (resource group) -> TreasureHuntGroupX (App Service)
		f. Select Finish
		g. Click the publish button in the upper right
Pops open the azure website or you can click on the url link in the resulting screen
