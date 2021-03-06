import { Component, OnInit, Input } from '@angular/core';
import { Photo } from 'src/app/_models/Photo';
import { FileUploader } from 'ng2-file-upload';
import { AuthService } from 'src/app/_services/auth.service';
import { environment } from 'src/environments/environment';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentMainPhoto: Photo;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertifyService: AlertifyService
  ) {}

  ngOnInit() {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url:
        this.baseUrl +
        'users/' +
        this.authService.decodedToken.nameid +
        '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1025 * 2024
    });

    this.uploader.onAfterAddingFile = file => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          description: res.description,
          dateAdded: res.dateAdded,
          isMain: res.isMain,
          url: res.url
        };

        this.photos.push(photo);
        if (photo.isMain) {
          this.authService.changeLoggedInUserPhoto(photo.url);
          const user = JSON.parse(localStorage.getItem('user'));
          user.photoUrl = photo.url;
          localStorage.setItem('user', JSON.stringify(user));
        }
      }
    };
  }

  setMain(photo: Photo) {
    this.userService
      .setMainPhoto(this.authService.decodedToken.nameid, photo.id)
      .subscribe(
        () => {
          this.currentMainPhoto = this.photos.filter(p => p.isMain === true)[0];
          this.currentMainPhoto.isMain = false;
          photo.isMain = true;
          this.authService.changeLoggedInUserPhoto(photo.url);
          const user = JSON.parse(localStorage.getItem('user'));
          user.photoUrl = photo.url;
          localStorage.setItem('user', JSON.stringify(user));
        },
        error => {
          this.alertifyService.error(error);
        }
      );
  }

  deletePhoto(photoId: number) {
    this.alertifyService.confirm(
      'Are you sure you want to delete this photo ?',
      () => {
        this.userService
          .deletePhoto(this.authService.decodedToken.nameid, photoId)
          .subscribe(
            () => {
              this.photos.splice(
                this.photos.findIndex(p => p.id === photoId),
                1
              );
              this.alertifyService.success('Photo has been deleted.');
            },
            error => {
              this.alertifyService.error(error);
            }
          );
      }
    );
  }
}
