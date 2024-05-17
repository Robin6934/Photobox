package com.PhotoboxWeb;

import java.io.IOException;
import java.util.Timer;
import java.util.TimerTask;

public class WatchdogTimer {
    private Timer timer;

    private int timerInterval = 5000;

    public WatchdogTimer(int timerInterval) {
        // Set the initial timer to 5 seconds
        timer = new Timer();
        this.timerInterval = timerInterval;
        timer.schedule(new WatchdogTask(), 0, timerInterval);
    }

    public void reset() {
        // Reset the timer to 5 seconds
        timer.cancel();
        timer = new Timer();
        timer.schedule(new WatchdogTask(), timerInterval);
    }

    private class WatchdogTask extends TimerTask {
        @Override
        public void run() {
            try {
                // Specify the path to the exe file
                String exePath = "C:\\Users\\Robin\\Documents\\GitHub\\CanonPhotoBooth\\CanonPhotoBooth\\bin\\Debug\\net8.0-windows\\CanonPhotoBooth.exe";

                // Create a process builder
                ProcessBuilder processBuilder = new ProcessBuilder(exePath);

                // Start the process
                Process process = processBuilder.start();

                // Wait for the process to finish (optional)
                int exitCode = process.waitFor();
                System.out.println("Exit Code: " + exitCode);
            } catch (IOException | InterruptedException e) {
                e.printStackTrace();
            }
        }
    }
}

