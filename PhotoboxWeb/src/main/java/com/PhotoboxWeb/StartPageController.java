package com.PhotoboxWeb;

import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;

import static com.PhotoboxWeb.PhotoBoothApplication.NewPictureName;
import static com.PhotoboxWeb.PhotoBoothApplication.ipAddress;

@Controller
public class StartPageController {

    @GetMapping("/")
    public String index(Model model) {

        model.addAttribute("imageName" , NewPictureName);
        model.addAttribute("ipAddress", ipAddress);

        return "StartPage";
    }
}
