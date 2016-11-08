# MindWave-EEG-Recorder
Records and graphs data from a NeuroSky MindWave EEG headset. Recording sessions can be used to help track your meditation, biohacking, and "quantified self" related experiments.

![MindWave Headset](http://by-jl.com/mindwave-eeg-recorder/readme/neurosky.jpg)

An electroencephalogram (EEG) is a test used to detect abnormalities related to electrical activity of the brain. This program is designed for use with the consumer grade headset NeuroSky MindWave.
This program graphs the recorded data as shown below. Throughout the duration of your test, you can enter comments to help see how specific aspects of your test impact the recorded brain waves. In the graph below, you can see that I made a comment saying I'm about to do math problems, and immediately following that comment we see a large deviation in the Red and Blue lines.

![MindWave Headset](http://by-jl.com/mindwave-eeg-recorder/readme/graph.png)

To see a live example of a test graph, [click here.](https://by-jl.com/mindwave-eeg-recorder/example/results.html)

##Getting Started

![MindWave Headset](http://by-jl.com/mindwave-eeg-recorder/readme/parts.jpg)

Ensure that the USB dongle is plugged in. Make sure that your headset is recognized by the MindWave Manager program, and that the ThinkGear Connector is running (two programs that come with your MindWave headset).

![MindWave Manager Screenshot](http://by-jl.com/mindwave-eeg-recorder/readme/mindwave-manager.png)

![ThinkGear Connector Screenshot](http://by-jl.com/mindwave-eeg-recorder/readme/mindwave-connector.png)

Follow the directions on the console program.

![Console Screenshot](http://by-jl.com/mindwave-eeg-recorder/readme/console.png)

When you are done with your test, type "end" into the Comment prompt. This exits the program and generates the dynamic graph, which is an HTML file located in Tests/YourTestName-YourTestDate/results.html