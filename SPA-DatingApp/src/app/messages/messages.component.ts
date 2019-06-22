import { Component, OnInit } from '@angular/core';
import { Pagination, PaginatedResult } from '../_models/pagination';
import { Message } from '../_models/message';
import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';


  constructor(private userService: UserService, private authService: AuthService,
              private route: ActivatedRoute, private alertify: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.messages = data.messages.result;
      this.pagination = data.messages.pagination;
    });
  }

    loadMessages() {
      this.userService.getMessages(this.authService.decodedToken.nameid, this.pagination.currentPage,
        this.pagination.itemsPerPage, this.messageContainer)
        .subscribe( (result: PaginatedResult<Message[]>) => {
          this.messages = result.result;
          this.pagination = result.pagination;
        }, error => {
          this.alertify.error(error);
        });
    }


    pageChanged(event: any): void {
      this.pagination.currentPage = event.page;
      this.loadMessages();
    }

    //
    deleteMessage(id: number) {
      this.alertify.confirm('Are you sure you want delete this message', () => {
        this.userService.deleteMessage(id, this.authService.decodedToken.nameid).subscribe(() => {
          // splice - metoda która usuwa i dodaje elementy
          // splice(indexDo usuniecia, ileElementow, zmienneKtoreZastapiaUsunieteElementy)
          // W tym wypadku ueuwamy wiadomość o indeksie zgodnym z id usuwanej wiadomości podawanej jako parametr
          // i usuwamy tylko jedną wiadomość - nic nie dodajemy wzamian
          this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
          this.alertify.success('Message has been deleted');
        }, error => {
          this.alertify.error('Failed to delete the message');
        });
      });
    }
}
