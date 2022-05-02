import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FileOutputComponent } from './file-output.component';

describe('FileOutputComponent', () => {
  let component: FileOutputComponent;
  let fixture: ComponentFixture<FileOutputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FileOutputComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FileOutputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
