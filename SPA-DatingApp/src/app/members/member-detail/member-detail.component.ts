import { Component, OnInit, ViewChild } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';
import { TabsetComponent } from 'ngx-bootstrap';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  // Dodanie dekoratora wykorzystujacego TabsetComponent z ngx-bootstrap pozwalający na nawigacje pomiędzy zakładkami
  // nie używając przycisków zakładek - w tym wypadku przejście do wiadomości przy użyciu dodatkowego przycisku
  @ViewChild('memberTabs') memberTabs: TabsetComponent;
  user: User;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  constructor(private userService: UserService, private alertify: AlertifyService,
              private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data.user;
    });

    // pobieranie danych dostarczonych w linku - w tym wypadku przeniesienie użytkownika
    // bezpośrednio do zakładki messages
    this.route.queryParams.subscribe(params => {
      const selectedTab = params.tab;
      // dodaj parametr "active" do zakładki pobranej z linku
      this.memberTabs.tabs[selectedTab > 0 ? selectedTab : 0].active = true;
    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];

    this.galleryImages = this.getImages();
  }

  getImages() {
    const imageUrls = [];
    // tslint:disable-next-line: prefer-for-of
    for (let i = 0; i < this.user.photos.length; i++) {
      imageUrls.push({
        small: this.user.photos[i].url,
        medium: this.user.photos[i].url,
        big: this.user.photos[i].url,
        descriptions: this.user.photos[i].description
      });
    }
    return imageUrls;
  }

  // dodaje status active konkretnej zakładce
  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }
}
