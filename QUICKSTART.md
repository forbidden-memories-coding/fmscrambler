## How to use the fmscrambler

After you have downloaded the current release from the GitHub Release section and extracted it's contents you will find an executable named ```FMScrambler.exe``` you need to run.

![fmscrambler start](https://i.imgur.com/Hh5hhc5.png)
> You will be greeted with the interface of the randomizer, like this.

* (1) Use the automatic extraction from an BIN/CUE image of the game via the `LOAD GAME (.CUE/.BIN)` button - **(!) RECOMMENDED**
* (1) You can also load the SLUS_014.11 and WA_MRG.MRG from an already unpacked image file of the game with the `LOAD SLUS/MRG` button
* (2) On the `SEED OPTIONS` you can enter your own numerical seed or use the `GENERATE RANDOM` button to generate a random seed.
* (3.1) Before you proceed further you might want to adjust the various randomization options via the `Randomizer` tab (See Image 2 for info)
* (3.2) After setting your preferred randomization options, go ahead and click the `RANDOMIZE!` button - It will take a short moment and notifies you after it's done
* (4) You are now ready to create a modified game image with the randomizations in place by clicking the `PATCH ISO` button - It will again show you a notice once it's done and automatically open a file explorer window where the patched game image is located

![fmscrambler settings](https://i.imgur.com/TS1yUA4.png)

* Randomize Attributes -> Randomize each card attribute
* Randomize Guardian Stars -> Randomize card Guardian Stars
* Allow Glitch Guardian Stars -> Allow glitchy Guardian Stars, like e.g. Equip
* Enable ATK/DEF scramble -> Randomize Attack and Defense of cards, set the range via the `ATK/DEF OPTIONS`
* "000" Glitch Fusions -> Enable glitch fusions for cards beyong 723
* Randomize Types -> Randomize card types (e.g. a monster can turn into a magic card)
* Randomize Card Drops -> Randomize card drops from duelists
* Randomize Duelist Decks -> Randomize duelist decks
* Randomize Equips -> Randomize what card takes what equip
* Randomize Fusions -> Randomize fusions

### **Some notes after randomization**
The randomized version of the game image will be called something like `FM_Randomizer_11-Dec-18_21-15-51` and consists of a `.BIN` and `.CUE` file. You want to load the `.CUE` file in your emulator of choice.
You will also find a file named like `scramblelog_#3947579.log` which is a detailed log on what got randomized in which way.
There will also be a folder named like `FM_Randomizer_11-Dec-18_21-15-51` which is created during the randomization. You can safely delete this folder and it's contents. In future versions of the randomizer this directory will get deleted automatically.