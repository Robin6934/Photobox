package com.PhotoboxWeb;

import com.PhotoboxWeb.DTOs.PolingDTO;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

import java.net.InetAddress;
import java.net.UnknownHostException;

@Controller
@RequestMapping("/PhotoBoothCommunication")
public class TriggerPictureController {

    private static boolean triggerPicture = false;

    private static String PrintPictureName = "";

    //WatchdogTimer watchdogTimer = new WatchdogTimer(5000);

    @GetMapping("/TriggerPictureHTML")
    private String TriggerPictureHTML(Model model)
    {
        String ipAdress = "";

        try {
            ipAdress = getIPAddress();
        } catch (UnknownHostException e) {
            e.printStackTrace();
        }

        model.addAttribute("ipAddress" , ipAdress);

        return "triggerPicture";
    }

    @ResponseBody
    @GetMapping("/TriggerPicture")
    private void TriggerPicture()
    {
        triggerPicture = true;
    }

    @ResponseBody
    @GetMapping("/PrintImage/{imageName}")
    private void PrintImage(@PathVariable String imageName)
    {
        imageName = imageName.replace("downscaled", "");

        PrintPictureName = imageName;
    }

    @ResponseBody
    @GetMapping("/Poling")
    private ResponseEntity<PolingDTO> TriggerPicturePoling()
    {
        PolingDTO polingDTO = new PolingDTO();

        //watchdogTimer.reset();

        polingDTO.setTriggerPicture(triggerPicture);
        polingDTO.setPrintPictureName(PrintPictureName);

        triggerPicture = false;
        PrintPictureName = "";

        return new ResponseEntity<>(polingDTO, HttpStatus.OK);
    }

    public static String getIPAddress() throws UnknownHostException {
        InetAddress localhost = InetAddress.getLocalHost();
        return localhost.getHostAddress();
    }


}
