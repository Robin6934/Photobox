package com.PhotoboxWeb;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.io.IOException;
import java.util.Timer;
import java.util.TimerTask;

@RestController
@RequestMapping("/PhotoBoothApi")
public class PhotoBoothRestAPIController {


    @GetMapping("/Init")
    private ResponseEntity<String> Init() throws IOException {
        return ResponseEntity.ok("Init Successful");
    }

    @GetMapping("/Shutdown")
    private ResponseEntity<String> Shutdown() throws IOException
    {
        scheduleShutdown();

        return ResponseEntity.ok("Shutdown Successful");
    }

    /**
     * Function to shut down the program with one-second delay
     */
    private static void scheduleShutdown() {
        Timer timer = new Timer();
        timer.schedule(new TimerTask() {
            @Override
            public void run() {
                // Perform any cleanup or necessary tasks before shutdown

                // Exit the application gracefully with an exit status code
                System.exit(0);
            }
        }, 1000); // Delay in milliseconds (5 seconds)
    }

}
