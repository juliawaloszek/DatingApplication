import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { error } from 'util';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(private userService: UserService, private authService: AuthService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  // Załadowanie wątku z wiadomościami, całej konwersacji
  loadMessages() {
    //sztuczka z plusem jeśli zrobimy jak poniżej (dodając +) to cuttertUserId będzie liczbą
    const currentUserId = +this.authService.decodedToken.nameid;
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
    .pipe(
      // tap = do || Kiedyś nazywało się to "do" jednak to słowo było już zarezerwane
      // tap wykonaj coś
      tap(messages => {
        // przejdziemy przez wszystkie wiadomości szukając tych ktore są nieotworzone przez obecnego użytkownika
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < messages.length; i++) {
          if (messages[i].isRead === false && messages[i].recipientId === currentUserId) {
              this.userService.markAsRead(currentUserId, messages[i].id);
          }
        }
      })
    )
    .subscribe(messages => {
      this.messages = messages;
    // tslint:disable-next-line: no-shadowed-variable
    }, error => {
      this.alertify.error(error);
    });
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage)
      .subscribe((message: Message) => {
        // tslint:disable-next-line: no-debugger
        // debugger;
        // dodanie nowej wiadomości na początek tablicy z wiadomościami
        this.messages.unshift(message);
        // wyczyszczenie pola wpisywania nowej wiadomości
        this.newMessage.content = '';
      // tslint:disable-next-line: no-shadowed-variable
      }, error => {
        this.alertify.error(error);
      } );
  }
}
