import { Spectator } from '@ngneat/spectator/jest';
import { By } from '@angular/platform-browser';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatSelectHarness } from '@angular/material/select/testing';

export function enterTextInInput<T>(component: Spectator<T>, inputId: string, text: string) {
  const input = component.debugElement.query(By.css(`#${inputId}`)).nativeElement;
  input.value = text;
  input.dispatchEvent(new Event('input'));

  component.detectChanges();
}

export function clickElementById<T>(component: Spectator<T>, elementId: string) {
  const element = component.debugElement.query(By.css(`#${elementId}`)).nativeElement;
  element.click();
  component.detectChanges();
}

export function clickElementByText<T>(component: Spectator<T>, text: string) {
  const element = queryElementByText(component, text).nativeElement;
  element.click();
  component.detectChanges();
}

export function clickSelectOption<T>(component: Spectator<T>, selectId: string, optionText: string) {
  clickElementById(component, selectId);

  const option = component.debugElement.query((e) => e.nativeElement.textContent === optionText).nativeElement;
  option.click();
  component.detectChanges();
}

export function queryElementByText<T>(component: Spectator<T>, text: string) {
  return component.debugElement.query((e) => e.nativeElement.textContent === text);
}

export function queryElementById<T>(component: Spectator<T>, id: string) {
  return component.debugElement.query(By.css(`#${id}`));
}

export function queryElementByCss<T>(component: Spectator<T>, css: string) {
  return component.debugElement.query(By.css(css));
}

export async function selectItemInSelect<T>(spectator: Spectator<T>, index) {
  const loader = TestbedHarnessEnvironment.loader(spectator.fixture);

  const select = await loader.getHarness(MatSelectHarness);
  await select.open();

  const options = await select.getOptions();
  await options[index].click();

  spectator.fixture.detectChanges();
}
