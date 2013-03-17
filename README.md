Pinultimate Windows Phone
=========================

Windows Phone 8 app for Team Pinultimate

## Setup

### Windows 8 Installation
1. Download Windows 8 (you **have** to use Windows 8)
   * Go here to download a free copy: http://msdn.stanford.edu/reqForm.php. Fill out the form, and **make sure you download the 64-bt version**
   * It might take a few hours before they email you with the download link, so don't worry if it doesn't happen right away
2. Download VMWare or VirtualBox
   * You can [download VMWare (Fusion, Player, or Workstation) for free here](http://e5.onthehub.com/WebStore/ProductsByMajorVersionList.aspx?cmi_mnuMain=0b57b739-b182-de11-8cd1-0030487d8897&ws=88485ab9-4fa4-dd11-a337-0030485a6b08) (scroll down to the bottom)
3. Configure Virtual Machine to support Windows Phone 8 Emulator
   * Follow the instructions in this article: http://www.developer.nokia.com/Community/Wiki/Windows_Phone_8_SDK_on_a_Virtual_Machine_with_Working_Emulator
   * __Make sure you enable Intel virtualization and you edit the .vmx file__

### Configure Environment on Windows 8

#### From here on, the remaining work should be done *in* the virtual machine

4. Download and Install Developer Tools
   * Install Windows Phone 8 SDK (includes Visual Studio 2012 and Emulator): https://dev.windowsphone.com/en-us/downloadsdk
   * Install GitHub for Windows: https://windows.github.com
5. Join the Windows Phone Dev Center (you can do this on the host OS)
   * Go here: https://dev.windowsphone.com/en-us/join and click "Join Now". You don't have to pay $99 dollars; just make sure you sign up/sign in with your stanford.edu email
6. Update the OS on your Nokia Lumia 822
  * __This is important. If you don't do this, you will not be able to install the app on your phone__
  * On your phone, go to settings > phone update > check for updates, and then accept the update. Should take 5-10 minutes and a couple restarts to install it
7. Register your Nokia Lumia 822 for Development
  * Plug in the phone into your computer (make sure that it connects to the Virtual Machine, not the host OS)
  * In Windows 8, open the Windows Phone Developer Registration app. (The easiest way to find it is simply to search for it.)
  * Follow the instructions from there, and the phone should now usable for development

Once these steps are complete, you should be able to clone the repo from GitHub, import it into Visual Studio 2012, and run the app either in the emulator or on the phone. If I missed anything or if you have any questions, lemme know!
