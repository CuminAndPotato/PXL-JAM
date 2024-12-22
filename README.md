<div align="center">
   <h1>🎄 Welcome to the PXL-JAM 2024🎄</h1>
</div>

🙏 This repository is proudly part of the [F# Advent Calendar 2024](https://sergeytihon.com/fsadvent/), hosted by Sergey Tihon - 🙏

The PXL PAM 2024 is a fun and engaging way to come together and have a joyful time!

- Learn programming - it's perfect for beginners and experienced developers alike.
- Team up and collaborate with other developers or take challenge for yourself.
- Share your creations
- And finally: You can **win a PXL-Clock Mk1** 🎉

<div align="center">
   <h2>🎁 <strong>Win a PXL-Clock - Watch How on YouTube</strong> 🎁</h2>
   <a href="https://youtu.be/q5-QTpEMGdU"><img src="https://img.youtube.com/vi/q5-QTpEMGdU/0.jpg" alt="Watch the PXL-JAM video" style="width: 60%;"></a>
   <p>YouTube</p>
</div>

## tl;dr - How does it work?

- Fork the Repo
- Program your own PXL-Apps
- Use the simulator to test your apps
- Submit your app via a pull request
- The creators of the 3 nicest app (by our jury preference) can win a PXL-Clock Mk1!

📣 **Spread the holiday cheer!**

Share this repository and help us make this a memorable event for the dotnet community.

## Terms and Conditions

All the information you can read in the README.md file and also in the [Questions and Answers](QuestionsAndAnswers.md) file is valid and part of the Terms and Conditions.
For more details, please refer to the [Terms and Conditions](TermsAndConditions.md) section.

## Questions and Answers

We have a separate [Questions and Answers](QuestionsAndAnswers.md) section that covers common queries about the PXL-JAM 2024. If you have any questions, check it out!

## Hands On!

To programm PXL-Apps, you need to set up your development environment. Here’s how to get started:

### Prerequisites

**Mandatory**

- [**.NET 8 SDK**](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [**Node / npm** - current LTS](https://nodejs.org)

**Optional (Recommended)**

- [**Visual Studio Code (VSCode)**](https://code.visualstudio.com/)
- [**Ionide-fsharp Extension for VSCode**](https://marketplace.visualstudio.com/items?itemName=Ionide.Ionide-fsharp)

### Fork the Repository

Best practice is to fork this repository to your GitHub account. This way, you can experiment with the code and save your changes, and maybe there will be some surprises along the way. 🎁

### Prepare the Sprites

Check out the sprites 🖼️ in `./PxlApps/assets` (like `pizzaMampf.png`) and swap them with your own custom artwork to personalize your app.

### Create Your First App

A PXL-App consists of two parts:
   - one or more F# script (or many of them) that contain the code for your app,
   - optionally some assets like images.

To set up your first app, follow these steps:

- Under the `./PxlApps/04_Submissions` folder, create a file with your GitHub account name and the name of your app

  Pattern: `{GHAccountName}_{UniqueAppName}.fsx`

  Example: `SchlenkR_MyFirstApp.fsx`

You can just copy and rename the example file `./PxlApps/04_Submissions/SchlenkR_MyFirstApp.fsx` to get started.

- If you have any assets (images), put them in the folder `./PxlApps/assets/submissions`- and name them:
  
  Pattern: `{GHAccountName}_{UniqueAppName}...`
  
  Example: `SchlenkR_MyImage.png`

### 🚀 Start the Simulator

Before running any apps, you’ll need to start the simulator.

⚠️ **Important:** Only one simulator should be running at a time.

1. Open the list of build tasks in VSCode:
   - Press `Ctrl+Shift+B` (Windows/Linux) or `Cmd+Shift+B` (macOS).
2. Select **Start Simulator** from the list.

As an alternative for the VSCode build task, just run `./start-simulator.sh` (Mac) or `./start-simulator.ps1` (Windows) in your terminal.

### Run an App

- Ensure the simulator is running (see above).
- Open your app file in the editor (works as well with all samples and tutorials here in this repo).
- Select the entire content of the file and run it by pressing `Alt+Enter` (Windows) or `Cmd+Enter` (Mac).

You can modify the code, open new files, and re-run apps as often as you like. Simply re-evaluate the **entire file** (that's the mose easy way.)

In case the simulator does not what you expect (e.g. you were in sleep mode), just restart the simulator.

### Submit Your App

When you’re ready to submit your app, create a pull request (PR) with your changes. We’ll review your app, provide feedback or merge it.

Follow-up PRs (updates) for your app in case you want to improve it are welcome until the end of the PXL-JAM 2024.

## Resources

### Tutorials and Examples

Explore the demo apps and tutorials in the `./PxlApps` directory.

### Programming Pitfalls

Have a look at the [./PxlApps/01_Basics/XX_Pitfalls.fsx](PxlApps/01_Basics/XX_Pitfalls.fsx) file to see some common pitfalls and how to avoid or fix them.


## Additional Information

**Resources**

- We offer videos and tutorials to help you set up everything.
- For learning F#
   - we recommend the [F# for Fun and Profit](https://fsharpforfunandprofit.com/) website.
   - A book by Ian Russel, available as eBook also: https://leanpub.com/essential-fsharp
   - "F# in Action" by Isaac Abraham: https://www.manning.com/books/f-sharp-in-action
- Getting the simulator up and running is straightforward.
- Examples are available to assist you in learning.

**Community Support**

- Don’t hesitate to ask for help!
- The community is friendly and eager to support fellow participants.

**Stay Updated**

- Follow Us: Keep up with the latest news by following us on Bluesky or similar platforms.
- Repository Announcements: Important updates will be posted in the repository.

We can’t wait to see your amazing creations! This is a fantastic opportunity to learn, experiment, and showcase your talent. Happy coding!

---

🎉 Enjoy experimenting!

🌟 Thank You for Being a Part of This Journey!

📣 If you like this PXL-JAM - please share it with others - thank you :)
