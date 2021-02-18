# Introduction
Anubis is a steganography library. Steganography is the art of hiding information in other media, for example pictures. Using Anubis, you can hide data in images and wave audio files. Anubis also allows you to retrive previously stored data in these forms of media.

Anubis uses the `Jector` to "inject", that is, embed data in the image, and "eject", that is, retrieve data from stored media.

# How it Works
Anubis stores data by modifying the LSB (Least Significant Bit) of each input byte to match the bit of the data being stored. For example, all bitmap images have pixels, which are point representations of specific color. Each pixel usually has three channels: Red, Green, Blue. PNG images also have the Alpha channel, which reflects to transparency. Each pixel channel is usually stored in a single byte, and the values of ARGB usually start at 0, to 255. Different colors are created by varying different values, for example, the color red has the following values: {R: 255, G: 0, B: 0}, while the color white has the values: {255, 255, 255}. The color purple looks something like {255, 0, 255}

With that in mind, we can look at how a byte usually looks like at the bit level. As you probably know, a `byte` is composed of 8 bits. Therefore, examine the byte provided below:

 1110 0111

This is the binary representation for the number 231. The Least Significant Bit is that bit, which if modified, has very little effect on the overall number itself. For example, modifying the last 1 to 0 (rightmost), to make 1110 0110 gives the number 230. 230 and 231 are not very far apart. The Most Significant bit however, is that bit, that if modified, has drastic effect on the number itself. For example, modifying the first 1 to 0 (leftmost) to give  0110 0111 gives the number 103. As you can see, 231 and 103 are far apart.

With this in mind, and returning to our pixel channels. Imagine changing the least significant bit for a specific channel. There should be very little difference between the overall output pixel and the original pixel. Consider the image below:

![LIB DIFFERENCE DEMONSTRATION](.\images\lsb-difference-demonstration.png "Significance of the LSB in pixels")

As you can see, there is no perceptible difference between these two images, though they both have different values of RGB.

Therefore, in storing data for bitmaps, the process used is as follows.
1. Read a byte from the data source, to get for example 0011 1001
2. Get 8 channels, that is 2 pixels with 3 channels each, and another two channels from the image, e.g 
    * 0010 1010
    * 1101 0100
    * 0111 1111
    * 0101 1010
    * 0101 1001
    * 1011 1101
    * 0100 0110
    * 1101 1010

3. Split the original data `byte` into difference bits, and modify each of the LSBs to store a single bit from the byte. So that each of the bytes are modified as shown below:
    * 0010 1010 -> 0010 101**0**
    * 1101 0100 -> 1101 010**0**
    * 0111 1111 -> 0111 111**1**
    * 0101 1010 -> 0101 101**1**
    * 0101 1001 -> 0101 100**1**
    * 1011 1101 -> 1011 110**0**
    * 0100 0110 -> 0100 011**0**
    * 1101 1010 -> 1101 101**1**

As is probably obvious, the LSB may not be changed at all on some occassions, making the final image even more difficult to percieve.

To obtain the hidden data, the reverese is true.

Look at the images below for an example of how good this method can hide data in images: Can you spot the original data and the image without the data?
<p float="left">
  <img src="images\x1.png" width="300"/>
  <img src="images\x2.png" width="300"/> 
</p>

# I'm now __EXCITED!__, where should I start?
Hold up, I know, I know, it's pretty briliant right? To get started, you can review my [tutorial](./Tutorial-CreateTextHidingApplication.md) on how to create a simple C# Console Application to hide text in an image. And in your excitement, don't forget that Anubis does not just do images, it also does .WAV audio files. The concept is the same btw for anyone wondering.