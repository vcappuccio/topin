# Topin

## Overview
Topin is a Windows Forms application that sets a low-level mouse hook to track mouse clicks. When the left mouse button is clicked, the application captures the cursor's X and Y coordinates and displays them in a label that follows the cursor. Additionally, the application provides a button to close the application and another button to capture a screenshot and analyze it using a Python script to send the screenshot to GTP-4o for analisys.

## Technologies Used
- .NET Framework 4.7.2
- Windows Forms
- System.Drawing
- System.Runtime.InteropServices
- System.Diagnostics
- OpenAI API (TODO - hence the python script)

## Setup and Installation
1. **Install Visual Studio:** Download and install Visual Studio 2022 or later with support for .NET desktop development.
2. **Clone the repository:** Clone the project repository to your local machine.
3. **Open the solution:** Open the `topin.sln` file in Visual Studio.

## Running the Project
- **From Visual Studio:**
  - Start Debugging (F5) or Start Without Debugging (Ctrl+F5)
  - This will build and run the application.

## Notes
- The application relies on P/Invoke to access Windows API functions for mouse hooking.
- The application uses a single form to display the click coordinates and the close button.
- **Important:** Ensure that a Python file named `analyze_image.py` should be placed in the same directory as where we are saving the screenshots, currently working on trying to get the dotnet-openai working.
