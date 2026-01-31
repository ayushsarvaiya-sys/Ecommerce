    import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { BulkImportProductsComponent } from './bulk-import-products.component';

describe('BulkImportProductsComponent', () => {
  let component: BulkImportProductsComponent;
  let fixture: ComponentFixture<BulkImportProductsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BulkImportProductsComponent, HttpClientTestingModule],
    }).compileComponents();

    fixture = TestBed.createComponent(BulkImportProductsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should validate file format', () => {
    const mockFile = new File(['test'], 'test.xlsx', { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    const event = { target: { files: [mockFile] } };
    component.onFileSelected(event);
    expect(component.selectedFile).toBe(mockFile);
  });

  it('should reject invalid file format', () => {
    const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });
    const event = { target: { files: [mockFile] } };
    component.onFileSelected(event);
    expect(component.selectedFile).toBeNull();
    expect(component.errorMessage).toBeTruthy();
  });

  it('should reset form', () => {
    component.selectedFile = new File(['test'], 'test.xlsx');
    component.resetForm();
    expect(component.selectedFile).toBeNull();
    expect(component.previewData).toBeNull();
    expect(component.importResult).toBeNull();
  });
});
