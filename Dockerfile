FROM openjdk:10

WORKDIR /opt/otp4gb
RUN mkdir -p /opt/otp4gbl
COPY otp-1.4.0-shaded.jar .
COPY graphs ./graphs

EXPOSE 8080

CMD [ "java", "-Xmx4G", \
  "-jar", "otp-1.4.0-shaded.jar", \
  "--router", "centredonsouthyorkshire", \
  "--graphs", "graphs", \
  "--server" ]