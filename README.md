# One-Way-Platforms
Replicating the way in which Dead Cells handles one way platforms while also considering a ledge hang. If you want all the features
like you would see in Dead Cells but also want to use your own scripts and aren't sure how to set that up, then watch the video I have
on my YouTube channel that goes over how to grab what you will need from my scripts and how to put them into your scripts; video coming very soon.

If you just want very basic one way platform abilities then all you will need to copy is the OneWayPlatform script and put it on
any GameObject that would act as a one way platform; there is some logic inside it that you might need to remove or adjust, so
take a look at the script if you don't want to watch the video and get tips on how to make it work in your project.

If you want one way platforms with the ability to hang from the ledge if ledge hanging would make sense, then copy the OneWayPlatform,
Ledge, and LedgeLocator scripts. As mentioned, attatch the OneWayPlatform and Ledge scripts to the platforms you want to behave as
one way platforms, and attach the LedgeLocator script to the player; default values for LedgeLocator are setup in script already, but
for a better understanding of how the LedgeLocator script works, visit my YouTube video that goes over in detail how everything works:
https://youtu.be/88NEMnzxwBs
