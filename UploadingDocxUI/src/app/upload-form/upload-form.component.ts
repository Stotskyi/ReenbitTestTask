import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { throwError } from 'rxjs';
import { FormBuilder, FormControl, FormGroup, NgForm, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-upload-form',
  templateUrl: './upload-form.component.html',
  styleUrls: ['./upload-form.component.css']
})
export class UploadFormComponent {
 
  status: "initial" | "uploading" | "success" | "fail" = "initial"; // Variable to store file status
  file: File | null = null; 
  email:string = "";
  fileError: string = '';
  constructor(private http: HttpClient,private formBuilder:FormBuilder) {}

  ngOnInit(): void {
  }
  mySubmit(form: NgForm) {
    if (form.valid) {
      this.email = form.value.email;
      console.log('User email:', this.email);
      this.onUpload();
    }
  }

  onChange(event: any) {
    const file: File = event.target.files[0];

    const allowedExtensions = /(\.docx)$/i;

    if (!allowedExtensions.exec(file.name)) {
      this.fileError = 'Only .docx files are allowed';
      console.log('File error:', this.fileError);
      this.file = null;
      event.target.value = ''; 
    } else {
        this.status = "initial";
        this.file = file;
        this.fileError = '';
    }
  }


  onUpload() {
    if (!this.file && !this.email) {
      console.error('File or email is missing.');
      return;
    }
    if (this.file) {
      const formData = new FormData();
      this.status = 'uploading';
      

      formData.append('file', this.file, this.file.name);
      formData.append('email',this.email)
      const upload$ = this.http.post("http://localhost:5210/api/Attachment", formData).subscribe(
        response => {
          console.log('Upload successful!', response);
          this.status = 'success';
        },
        error => {
          console.error('Error uploading file:', error);
          this.status = 'fail';
        }
      );;
    }
  }
}

